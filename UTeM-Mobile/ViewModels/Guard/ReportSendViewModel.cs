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

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ReportSendViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Report> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<Patrol> _genericPatrolService;
        private bool hasPatrol;
        private bool hasNoPatrol;
        private Report report;
        private FileResult photoResult;
        private ApplicationUser user;
        private Patrol patrol;

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

        public ReportSendViewModel()
        {
            Report = new Report();
            _genericService = new GenericService<Report>();
            _genericUserService = new GenericService<ApplicationUser>();
            _genericPatrolService = new GenericService<Patrol>();
            TakePhotoCommand = new AsyncCommand(TakePhotoAsync);
            SendReportCommand = new AsyncCommand(ExecuteSendReport);
        }

        private async Task TakePhotoAsync()
        {
            try
            {
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
                    //await GenerateFileMessage();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ExecuteSendReport()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Report.Description))
                {
                    IsSuccessMessage = false;
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
                requestContent.Add(new StringContent(token.UserId), "guardId");
                requestContent.Add(new StringContent(Patrol.Id.ToString()), "patrolId");
                requestContent.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "date");
                ObjectResponse<Report> response = await _genericService.PostFile(url, requestContent, token);
                IsSuccessMessage = response.IsSuccess;
                Message = response.Message;
                if (IsSuccessMessage)
                {
                    Report = new Report();
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "SoS notification" },
                                        { "Title", response.Message },
                                        { "Message", "" },
                                        { "NavigateTo", "" },
                                        { "HasNavigate", "false" }
                                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch(Exception ex)
            {
                IsSuccessMessage = false;
                Message = "Unexpected error occured!";
            }
            finally
            {
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            token = await LocalDBService.GetToken();
            if (token != null)
            {
                await GetProfileAsync();
                await GetUserPatrol();
            }
        }

        private async Task GetProfileAsync()
        {
            string url = "accounts/me";
            ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
            User = response.Data;
        }
        private async Task GetUserPatrol()
        {
            try
            {
                string url = "patrols/status";
                ObjectResponse<Patrol> response = await _genericPatrolService.PostAsync(url, null, token);
                Patrol = response.Data;
                HasPatrol = response.Data != null && response.Data.Status == "Started";
            }
            catch(Exception ex)
            {

            }
        }
    }
}
