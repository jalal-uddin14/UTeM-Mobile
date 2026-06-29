using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class ProfileViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericService;
        
        private readonly ITokenStorageService _tokenService;
        private readonly ILogoutService _logoutService;

        private ApplicationUser user;

        public ICommand UpdateProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public ProfileViewModel(ITokenStorageService tokenService, ILogoutService logoutService)
        {
            _tokenService = tokenService;
            _logoutService = logoutService;
            User = new ApplicationUser();
            _genericService = new GenericService<ApplicationUser>();
            UpdateProfileCommand = new AsyncCommand(ExecuteUpdateProfile);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }
        private async Task ExecuteLogout()
        {
            try
            {
                await _logoutService.LogoutAsync();
            }
            catch (Exception)
            {

            }
        }
        private async Task ExecuteUpdateProfile()
        {
            try
            {
                string url = "accounts/update";
                ObjectResponse<ApplicationUser> response = await _genericService.PutAsync(url, User, Token);
                IsErrorMessage = !response.IsSuccess;
                if (IsSuccessMessage)
                {
                    Message = response.Message;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Error", "Internal error occured", response.Message)))
                    );
                }
            }
            catch (Exception)
            {
                IsErrorMessage = true;
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
            finally
            {
                await GetProfileAsync();
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }

        public async Task OnAppearing()
        {
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            var tokenJson = await _tokenService.GetAccessTokenAsync();
            if (tokenJson != null)
            {
                await GetProfileAsync();
            }
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    User = response.Data;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Error", "Internal error occured", response.Message)))
                    );
                }
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
        }
    }
}
