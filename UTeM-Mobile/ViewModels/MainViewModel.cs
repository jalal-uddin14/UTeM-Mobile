using MvvmHelpers;
using System.Collections.ObjectModel;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Services;

namespace UTeM_Mobile.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private AuthToken token;
        private bool isSuccessMessage;
        private bool isErrorMessage;
        private string message;
        private int errorHeight;
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
        public ObservableCollection<ErrorView> Message_list { get; }
        public int ErrorHeight { get => errorHeight; set => SetProperty(ref errorHeight, value); }
        protected AuthToken Token
        {
            get => token;
            set
            {
                SetProperty(ref token, value);
                if (value.ValidTo <= DateTime.Now)
                {
                    Task.Run(async () =>
                    {
                        await LogoutService.LogoutAsync();
                    });
                }
            }
        }

        public MainViewModel()
        {
            Message_list = new ObservableCollection<ErrorView>();
            ErrorHeight = 0;
            IsErrorMessage = false;
        }

        protected void SetErrorMessage(string message, Dictionary<string, List<string>> errors = null)
        {
            IsErrorMessage = true;
            Message_list.Clear();
            if (message != null)
            {
                Message_list.Add(new ErrorView { Message = message, Color = "Red" });
                ErrorHeight += 60;
            }
            if (errors != null && errors.Count > 0)
            {
                foreach (var m in errors)
                {
                    Message_list.Add(new ErrorView { Message = m.Value[0], Color = "Red" });
                }
                ErrorHeight += errors.Count * 20;
            }
        }
    }
}
