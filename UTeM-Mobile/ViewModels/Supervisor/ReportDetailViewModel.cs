using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class ReportDetailViewModel : BaseViewModel
    {
        private IGenericService<Report> _genericService;
        private int id;
        private Report report;
        private AuthToken token;

        public int Id { get => id; set => id = value; }
        public Report Report { get => report; set => SetProperty(ref report, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public ReportDetailViewModel()
        {
            _genericService = new GenericService<Report>();
            Report = new Report
            {
                Patrol = new Patrol
                {
                    Route = new Route()
                }
            };
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetReportDetail();
            }
        }

        private async Task GetReportDetail()
        {
            try
            {
                string url = "reports/" + Id;
                ObjectResponse<Report> response = await _genericService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    Report = response.Data;
                    Report.FilePath = ServerCredential.BaseUrl + "reports/files/" + Report.File;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                      Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetNavigationMessage("SoS", "SoS Error", "ReportListPage", "Failed to get SoS Data")))
                  );
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
        }
    }
}
