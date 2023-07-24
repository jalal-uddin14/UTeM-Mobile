using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http.Headers;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ReportSendViewModel : BaseViewModel, IOnAppearing
    {
        private IGenericService<Report> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<Patrol> _genericPatrolService;
        private Report report;
        private FileResult photoResult;
        private AuthToken token;
        private ApplicationUser user;
        private Patrol patrol;

        public ICommand TakePhotoCommand { get; }
        public ICommand SendReportCommand { get; }

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
            string url = "reports";
            var requestContent = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(File.ReadAllBytes(PhotoResult.FullPath));
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            requestContent.Add(imageContent, "file", "image.jpg");
            requestContent.Add(new StringContent(Report.Description), "Description");
            requestContent.Add(new StringContent(token.UserId), "guardId");
            requestContent.Add(new StringContent(Patrol.Id.ToString()), "patrolId");
            requestContent.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "date");
            ObjectResponse<Report> response = await _genericService.PostFile(url, requestContent, token);
            var report = response.Data;
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
            string url = "patrols/status";
            ObjectResponse<Patrol> response = await _genericPatrolService.InsertAsync(url, null, token);
            Patrol = response.Data;
        }
    }
}
