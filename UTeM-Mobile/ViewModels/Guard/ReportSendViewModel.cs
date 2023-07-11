using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RestSharp;
using System.Net.Http.Headers;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ReportSendViewModel : BaseViewModel
    {
        private IGenericService<Report> _genericService;
        private Report report;
        private FileResult photoResult;

        public ICommand TakePhotoCommand { get; }
        public ICommand SendReportCommand { get; }

        public Report Report { get => report; set => SetProperty(ref report, value); }
        public FileResult PhotoResult { get => photoResult; set => SetProperty(ref photoResult, value); }

        public ReportSendViewModel()
        {
            Report = new Report();
            _genericService = new GenericService<Report>();
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
            //    here you can specify boundary if you need---^
            var imageContent = new ByteArrayContent(File.ReadAllBytes(PhotoResult.FullPath));
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            requestContent.Add(imageContent, "file", "image.jpg");
            requestContent.Add(new StringContent(Report.Description), "Description");
            requestContent.Add(new StringContent("6c3bb649-f48e-470f-8475-bbab6fd7cfcf"), "guardId");
            requestContent.Add(new StringContent("1"), "patrolId");
            requestContent.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "date");
            ObjectResponse<Report> response = await _genericService.PostFile(url, requestContent);
            var report = response.Data;
        }

        public void OnAppearing()
        {

        }
    }
}
