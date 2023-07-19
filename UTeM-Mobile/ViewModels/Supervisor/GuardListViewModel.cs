using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class GuardListViewModel : BaseViewModel
    {
        private IGenericService<ApplicationUser> _genericService;
        private AuthToken token;
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }
        public GuardListViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
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
                await GetGuardList();
            }
        }

        private async Task GetGuardList()
        {
            string route = "guards";
            PaginatedResponse<ApplicationUser> response = await _genericService.GetPagedListAsync(route, Token);
            if (response.IsSuccess)
            {
                GuardList.Clear();
                GuardList.AddRange(response.Data.Data);
            }
        }
    }
}
