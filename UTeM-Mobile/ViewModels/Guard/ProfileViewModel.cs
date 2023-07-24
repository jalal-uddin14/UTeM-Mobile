using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ProfileViewModel : BaseViewModel, IOnAppearing
    {
        private AuthToken token;
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;

        public ICommand LogoutCommand { get; }

        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public ProfileViewModel()
        {
            _genericUserService = new GenericService<ApplicationUser>();
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }
        private async Task ExecuteLogout()
        {
            try
            {
                await LocalDBService.RemoveToken();
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            token = await LocalDBService.GetToken();
            if (token != null)
            {
                await GetUserDetailAsync();
            }
        }

        private async Task GetUserDetailAsync()
        {
            string url = "accounts/me";
            ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
            if (response != null)
            {
                User = response.Data;
            }
        }
    }
}
