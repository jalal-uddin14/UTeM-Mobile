using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class GuardDetailViewModel : BaseViewModel
    {
        private IGenericService<ApplicationUser> _genericService;
        private string id;
        private ApplicationUser guard;
        private AuthToken token;

        public string Id { get => id; set => id = value; }
        public ApplicationUser Guard { get => guard; set => SetProperty(ref guard, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public GuardDetailViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            Guard = new ApplicationUser();
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
                await GetGuardDetail();
            }
        }

        private async Task GetGuardDetail()
        {
            string url = "guards/" + Id;
            ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
            Guard = response.Data;
        }
    }
}
