using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels
{
    public partial class LoginViewModel : MainViewModel
    {
        [ObservableProperty]
        private bool isRemember;

        [ObservableProperty]
        private AuthToken authToken = new();

        [ObservableProperty]
        private ApplicationUser user;

        private readonly IAuthenticationService _authenticationService;
        private readonly ILoginFlowService _loginFlowService;

        public LoginViewModel(IAuthenticationService authentication, ILoginFlowService loginFlow)
        {
            _authenticationService = authentication;
            User = _authenticationService.CurrentUser ?? new ApplicationUser();
            _loginFlowService = loginFlow;
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
                await _loginFlowService.LoginAsync(User, IsRemember);
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
            }
            finally
            {
                IsBusy = false;
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }
    }
}
