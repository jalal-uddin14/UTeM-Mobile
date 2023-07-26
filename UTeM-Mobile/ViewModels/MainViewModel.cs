using MvvmHelpers;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        protected AuthToken token;
        private bool isSuccessMessage;
        private bool isErrorMessage;
        private string message;
        public bool IsSuccessMessage
        {
            get => isSuccessMessage;
            set
            {
                SetProperty(ref isSuccessMessage, value);
                IsErrorMessage = !value;
            }
        }
        public bool IsErrorMessage { get => isErrorMessage; set => SetProperty(ref isErrorMessage, value); }
        public string Message { get => message; set => SetProperty(ref message, value); }
    }
}
