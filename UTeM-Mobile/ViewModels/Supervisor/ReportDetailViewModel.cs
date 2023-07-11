using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class ReportDetailViewModel : BaseViewModel
    {
        private IGenericService<Report> _genericService;
        private int id;
        private Report report;

        public int Id { get => id; set => id = value; }
        public Report Report { get => report; set => SetProperty(ref report, value); }

        public ReportDetailViewModel()
        {
            _genericService = new GenericService<Report>();
            Report = new Report();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetReportDetail(); });
        }

        private async Task GetReportDetail()
        {
            string url = "reports/" + Id;
            ObjectResponse<Report> response = await _genericService.GetDetailsAsync(url);
            Report = response.Data;
            //Report.FilePath = ServerCredential.BaseUrl + "reports/files/" + Report.File;
            Report.FilePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS7FB0RcV2PQHhD0kuwIWEAXkrAVGT74EoieA&usqp=CAU";
        }
    }
}
