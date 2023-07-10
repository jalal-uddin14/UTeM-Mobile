using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class GuardListViewModel : BaseViewModel
    {
        private string _baseUrl;
        private IGenericService<ApplicationUser> _genericService;
        private ApplicationUser LoggedinUser;
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public GuardListViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetGuardList(); });
        }

        private async Task GetGuardList()
        {
            string route = "guards";
            PaginatedResponse<ApplicationUser> response = await _genericService.GetPagedListAsync(route);
            if (response.IsSuccess)
            {
                GuardList.Clear();
                GuardList.AddRange(response.Data.Data);
            }
        }
    }
}
