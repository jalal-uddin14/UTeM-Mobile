using MvvmHelpers.Commands;
using Plugin.NFC;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;

namespace UTeM_Mobile.ViewModels
{
    public class LoginViewModel : MainViewModel
    {
        private bool isRemember;
        private string errorMessage;
        private AuthToken authToken;
        private ApplicationUser user;
        private IGenericService<AuthToken> _authService;

        public ICommand LoginCommand { get; }
        public bool IsRemember { get => isRemember; set => SetProperty(ref isRemember, value); }
        public string ErrorMessage { get => errorMessage; set => SetProperty(ref errorMessage, value); }
        public AuthToken AuthToken { get => authToken; set => SetProperty(ref authToken, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public LoginViewModel()
        {
            AuthToken = new AuthToken();
            User = new ApplicationUser();
            _authService = new GenericService<AuthToken>();
            LoginCommand = new AsyncCommand(ExecuteLogin);
        }

        private async Task ExecuteLogin()
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
                ObjectResponse<AuthToken> response = await _authService.PostAsync(url, User);
                if (response.IsSuccess && response.Data != null)
                {
                    AuthToken = response.Data;
                    AuthToken.ValidTo = DateTime.Now.AddMinutes(response.Data.LifetimeMinutes);
                    AuthToken.IsRemember = IsRemember;
                    await LocalDBService.RemoveToken();
                    await PatrolDBService.Delete();
                    await LocalDBService.InsertToken(AuthToken);
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
                            NFCService.SubscribeNFC();
                        }
                        await TimeOutService.CheckTimerToken();
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

        public void OnAppearing()
        {

        }
    }
}
