using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class ReportListViewModel : BaseViewModel
    {
        private IGenericService<Report> _genericService;

        public ObservableRangeCollection<Report> ReportList { get; set; }

        public ReportListViewModel()
        {
            _genericService = new GenericService<Report>();
            ReportList = new ObservableRangeCollection<Report>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetReportList(); });
        }

        private async Task GetReportList()
        {
            string url = "reports";
            PaginatedResponse<Report> response = await _genericService.GetPagedListAsync(url);
            ReportList.Clear();
            ReportList.AddRange(response.Data.Data);
        }
    }
}
