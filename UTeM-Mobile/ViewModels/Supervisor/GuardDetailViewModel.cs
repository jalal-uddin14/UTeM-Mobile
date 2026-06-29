using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class GuardDetailViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericService;
        private readonly ITokenStorageService _tokenService;
        private string id;
        private ApplicationUser guard;

        public string Id { get => id; set => id = value; }
        public ApplicationUser Guard { get => guard; set => SetProperty(ref guard, value); }

        public GuardDetailViewModel(ITokenStorageService tokenService)
        {
            _genericService = new GenericService<ApplicationUser>();
            Guard = new ApplicationUser();
            _tokenService = tokenService;
        }

        public async Task OnAppearing()
        {
            IsErrorMessage = false;
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (tokenJson != null)
                {
                    await GetGuardDetail();
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured");
            }
        }

        private async Task GetGuardDetail()
        {
            try
            {
                string url = "guards/" + Id;
                ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
                if (response != null && response.Data != null)
                {
                    Guard = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured");
            }
        }
    }
}
