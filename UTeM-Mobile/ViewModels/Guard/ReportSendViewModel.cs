using MvvmHelpers.Commands;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http.Headers;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Services;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ReportSendViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Report> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private bool hasPatrol;
        private bool hasNoPatrol;
        private Report report;
        private FileResult photoResult;
        private ApplicationUser user;
        private Patrol patrol;
        private Checkpoint checkpoint;
        private PatrolCheckpoint patrolCheckpoint;

        public ICommand TakePhotoCommand { get; }
        public ICommand SendReportCommand { get; }

        public bool HasPatrol
        {
            get => hasPatrol;
            set
            {
                SetProperty(ref hasPatrol, value);
                HasNoPatrol = !value;
            }
        }
        public bool HasNoPatrol { get => hasNoPatrol; set => SetProperty(ref hasNoPatrol, value); }
        public Report Report { get => report; set => SetProperty(ref report, value); }
        public FileResult PhotoResult { get => photoResult; set => SetProperty(ref photoResult, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public Checkpoint Checkpoint { get => checkpoint; set => SetProperty(ref checkpoint, value); }
        public PatrolCheckpoint PatrolCheckpoint { get => patrolCheckpoint; set => SetProperty(ref patrolCheckpoint, value); }

        public ReportSendViewModel()
        {
            Report = new Report();
            _genericService = new GenericService<Report>();
            _genericUserService = new GenericService<ApplicationUser>();
            TakePhotoCommand = new AsyncCommand(TakePhotoAsync);
            SendReportCommand = new AsyncCommand(ExecuteSendReport);
        }

        private async Task TakePhotoAsync()
        {
            try
            {
                IsSuccessMessage = false;
                if (CrossMedia.Current.IsTakePhotoSupported)
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        CompressionQuality = 20
                    });

                    if (file != null)
                    {
                        PhotoResult = new FileResult(file.Path);
                        var a = file.GetStream().ReadByte();
                        var b = File.ReadAllBytes(PhotoResult.FullPath);
                    }
                }
                else
                {
                    SetErrorMessage("Camera not supported");
                }
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task ExecuteSendReport()
        {
            try
            {
                IsSuccessMessage = false;
                if (string.IsNullOrWhiteSpace(Report.Description))
                {
                    IsErrorMessage = true;
                    Message = "SoS Message required";
                    return;
                }
                string url = "reports";
                var requestContent = new MultipartFormDataContent();
                if (PhotoResult != null)
                {
                    var imageContent = new ByteArrayContent(File.ReadAllBytes(PhotoResult.FullPath));
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    requestContent.Add(imageContent, "file", "image.jpg");
                }
                requestContent.Add(new StringContent(Report.Description), "Description");
                requestContent.Add(new StringContent(Token.UserId), "guardId");
                requestContent.Add(new StringContent(PatrolCheckpoint.Id.ToString()), "patrolCheckpointId");
                requestContent.Add(new StringContent(DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")), "date");
                ObjectResponse<Report> response = await _genericService.PostFile(url, requestContent, Token);
                IsErrorMessage = !response.IsSuccess;
                Message = response.Message;
                if (IsSuccessMessage)
                {
                    PhotoResult = null;
                    Report = new Report();
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("SoS notification", response.Message)))
                    );
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("SoS notification", "Error occured", response.Message)))
                    );
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Unexpected error occured!");
            }
            finally
            {
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }

        public void OnAppearing()
        {
            IsErrorMessage = false;
            Report = new Report();
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                HasPatrol = false;
                Checkpoint = null;
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
                    await GetProfileAsync();
                    await GetUserPatrol();
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Unexpected error occured!");
            }
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    User = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Unexpected error occured!");
            }
        }
        private async Task GetUserPatrol()
        {
            try
            {
                IsErrorMessage = false;
                ObjectResponse<Patrol> patrolResponse = await PatrolService.GetPatrolStatus();
                if (patrolResponse.IsSuccess)
                {
                    if (patrolResponse.Data != null && patrolResponse.Data.Status == "Started" && patrolResponse.Data.PatrolCheckpoints.Count > 0 && patrolResponse.Data.PatrolCheckpoints.FirstOrDefault(q => q.Status == "Scheduled") != null && patrolResponse.Data.PatrolCheckpoints.FirstOrDefault(q => q.Status == "Scheduled").Checkpoint != null)
                    {
                        //Patrol = patrolResponse.Data;
                        PatrolCheckpoint = patrolResponse.Data.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                        //Checkpoint = patrolResponse.Data.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled").Checkpoint;
                    }
                }
                else
                {
                    IsErrorMessage = true;
                    SetErrorMessage(patrolResponse.Message, patrolResponse.Errors);
                }
                HasPatrol = PatrolCheckpoint != null;
                
            }
            catch(Exception)
            {
                IsErrorMessage = true;
                SetErrorMessage("Unexpected error occured!");
            }
        }
    }
}
