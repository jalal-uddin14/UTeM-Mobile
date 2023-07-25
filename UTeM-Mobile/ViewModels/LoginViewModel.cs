using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool isRemember;
        private bool isError;
        private string errorMessage;
        private AuthToken authToken;
        private ApplicationUser user;
        private IGenericService<AuthToken> _authService;

        public ICommand LoginCommand { get; }
        public bool IsRemember { get => isRemember; set => SetProperty(ref isRemember, value); }
        public bool IsError { get => isError; set => SetProperty(ref isError, value); }
        public string ErrorMessage { get => errorMessage; set => SetProperty(ref errorMessage, value); }
        public AuthToken AuthToken { get => authToken; set => SetProperty(ref authToken, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public LoginViewModel()
        {
            IsError = false;
            AuthToken = new AuthToken();
            User = new ApplicationUser();
            _authService = new GenericService<AuthToken>();
            LoginCommand = new AsyncCommand(ExecuteLogin);
        }

        private async Task ExecuteLogin()
        {
            try
            {
                IsError = false;
                string url = "accounts/login";
                ObjectResponse<AuthToken> response = await _authService.InsertAsync(url, User);
                if (response.IsSuccess && response.Data != null)
                {
                    AuthToken = response.Data;
                    AuthToken.ValidTo = DateTime.Now.AddMinutes(response.Data.LifetimeMinutes);
                    AuthToken.IsRemember = IsRemember;
                    await LocalDBService.InsertToken(AuthToken);
                    if (response.Data.UserRole == "Supervisor")
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage = new SupervisorShell();
                        });
                    }
                    else if (response.Data.UserRole == "Guard")
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage = new GuardShell();
                        });
                    }
                }
                else
                {
                    IsError = true;
                    ErrorMessage = response.Message;
                }
            }
            catch (Exception ex)
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
