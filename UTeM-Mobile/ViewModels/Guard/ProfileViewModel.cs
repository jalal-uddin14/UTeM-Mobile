using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ProfileViewModel : MainViewModel, IOnAppearing
    {
        private AuthToken token;
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;

        public ICommand LogoutCommand { get; }
        public ICommand UpdateProfileCommand { get; }

        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public ProfileViewModel()
        {
            _genericUserService = new GenericService<ApplicationUser>();
            UpdateProfileCommand = new AsyncCommand(ExecuteUpdateProfile);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }
        private async Task ExecuteUpdateProfile()
        {
            try
            {
                string url = "accounts/update";
                ObjectResponse<ApplicationUser> response = await _genericUserService.PutAsync(url, User, token);
                IsSuccessMessage = response.IsSuccess;
                Message = response.Message;
                await GetUserDetailAsync();
            }
            catch (Exception ex)
            {
                IsSuccessMessage = false;
                Message = "Unexpected error occured!";
            }
            finally
            {
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
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
