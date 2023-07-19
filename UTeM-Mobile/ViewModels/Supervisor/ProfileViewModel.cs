using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class ProfileViewModel : BaseViewModel
    {
        private IGenericService<ApplicationUser> _genericService;
        private ApplicationUser user;
        private AuthToken token;

        public ICommand UpdateProfileCommand { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public ProfileViewModel()
        {
            User = new ApplicationUser();
            _genericService = new GenericService<ApplicationUser>();
            UpdateProfileCommand = new AsyncCommand(ExecuteUpdateProfile);
        }

        private async Task ExecuteUpdateProfile()
        {
            try
            {
                string url = "accounts/update";
                ObjectResponse<ApplicationUser> response = await _genericService.UpdateAsync(url, User, Token);
            }
            catch (Exception ex)
            {

            }
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
                await GetProfileAsync();
            }
        }

        private async Task GetProfileAsync()
        {
            string url = "accounts/me";
            ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
            User = response.Data;
        }
    }
}
