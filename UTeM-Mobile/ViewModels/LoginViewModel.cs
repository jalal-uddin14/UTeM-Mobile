using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ApplicationUser user;
        private bool isErrorMessage;
        private IGenericService<AuthToken> _authService;

        public ICommand LoginCommand { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public bool IsErrorMessage { get => isErrorMessage; set => SetProperty(ref isErrorMessage, value); }

        public LoginViewModel()
        {
            User = new ApplicationUser();
            _authService = new GenericService<AuthToken>();
            LoginCommand = new AsyncCommand(ExecuteLogin);
        }

        private async Task ExecuteLogin()
        {
            string url = "supervisors/login";
            ObjectResponse<AuthToken> response = await _authService.InsertAsync(url, User);
            if (response.IsSuccess)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new SupervisorShell();
                });
            }
        }
    }
}
