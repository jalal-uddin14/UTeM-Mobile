using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.NFC;
using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.ViewModels
{
    public partial class LoginViewModel : MainViewModel
    {
        [ObservableProperty]
        private bool isRemember;

        [ObservableProperty]
        private AuthToken authToken;

        [ObservableProperty]
        private ApplicationUser user;

        private readonly IGenericService<AuthToken> _authService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenStorageService _tokenService;
        private readonly INFCService _nfcService;
        private readonly ITimeOutService _timeOutService;

        public LoginViewModel(IGenericService<AuthToken> authService, IAuthenticationService authenticationService, ITokenStorageService tokenService, INFCService nfcService, ITimeOutService timeOutService)
        {
            _authService = authService;
            _authenticationService = authenticationService;
            User = _authenticationService.CurrentUser ?? new ApplicationUser();
            AuthToken = _authenticationService.CurrentToken;
            _tokenService = tokenService;
            _nfcService = nfcService;
            _timeOutService = timeOutService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                IsErrorMessage = false;
                Message_list.Clear();
                ErrorHeight = 0;
                if (User.Email == null || User.Email == "")
                {
                    SetErrorMessage("Email is required");
                    return;
                }
                if (User.Password == null || User.Password == "")
                {
                    SetErrorMessage("Password is required");
                    return;
                }
                IsBusy = true;
                string url = "accounts/login";
                ObjectResponse<AuthToken> response = await _authService.PostAsync(url, user);
                if (response.IsSuccess && response.Data != null)
                {
                    AuthToken = response.Data;
                    AuthToken.ValidTo = DateTime.UtcNow.AddHours(8).AddMinutes(response.Data.LifetimeMinutes);
                    AuthToken.IsRemember = IsRemember;
                    _tokenService.RemoveAccessToken();
                    await _tokenService.SaveAccessTokenAsync(JsonSerializer.Serialize(AuthToken));
                    StaticCredentials.CheckpointTimer = null;
                    if (response.Data.UserRole == "Supervisor")
                    {
                        await PusherService.SubscribeGuardChannel();
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage = new SupervisorShell();
                        });
                    }
                    else if (response.Data.UserRole == "Guard")
                    {
                        if (!CrossNFC.Current.IsAvailable)
                        {
                            await App.Current.MainPage.DisplayAlert("Failed", "NFC is not available in your phone.", "OK");
                        }
                        else if (!CrossNFC.Current.IsEnabled)
                        {
                            await App.Current.MainPage.DisplayAlert("Failed", "NFC is not active in your phone.", "OK");
                        }
                        else
                        {
                            _nfcService.SubscribeNFC();
                        }
                        await _timeOutService.CheckTimerToken();
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage = new GuardShell();
                        });
                    }
                }
                else
                {
                    IsBusy = false;
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }
    }
}
