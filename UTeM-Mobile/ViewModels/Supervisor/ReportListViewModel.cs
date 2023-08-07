using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using System.Windows.Input;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class ReportListViewModel : BaseViewModel
    {
        private IGenericService<Report> _genericService;
        private AuthToken token;

        public ICommand NavigateToDetailCommand { get; }

        public ObservableRangeCollection<Report> ReportList { get; set; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public ReportListViewModel()
        {
            _genericService = new GenericService<Report>();
            ReportList = new ObservableRangeCollection<Report>();
        }

        public void OnAppearing()
        {
            IsBusy = true;
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetReportList();
            }
        }

        private async Task GetReportList()
        {
            try
            {
                ReportList.Clear();
                string url = "reports";
                PaginatedResponse<Report> response = await _genericService.GetPagedListAsync(url, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    ReportList.AddRange(response.Data.Data);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Error", "Internal error occured", response.Message)))
                    );
                }

            }
            catch(Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
