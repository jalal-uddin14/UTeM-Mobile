using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class ProfileViewModel : MainViewModel, IOnAppearing
    {
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
                IsErrorMessage = false;
                string url = "accounts/update";
                ObjectResponse<ApplicationUser> response = await _genericUserService.PutAsync(url, User, Token);
                IsSuccessMessage = response.IsSuccess;
                if (IsSuccessMessage)
                {
                    Message = response.Message;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Error", "Internal error occured", response.Message)))
                    );
                    SetErrorMessage("Internal error occured.");
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                await GetUserDetailAsync();
            }
        }
        private async Task ExecuteLogout()
        {
            try
            {
                IsErrorMessage = false;
                await LogoutService.LogoutAsync();
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        public void OnAppearing()
        {
            IsErrorMessage = false;
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
                    await GetUserDetailAsync();
                }
            }
            catch(Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetUserDetailAsync()
        {
            try
            {
                IsErrorMessage = false;
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    User = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }
    }
}
