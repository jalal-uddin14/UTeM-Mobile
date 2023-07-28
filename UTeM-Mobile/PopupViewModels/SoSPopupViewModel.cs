using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.PopupViewModels
{
    public class SoSPopupViewModel : MainViewModel
    {
        private Dictionary<string, string> notification;
        private string notificationMessage;
        private string description;

        public ICommand NavigateToSoSListCommand { get; }
        public ICommand ClosePopViewCommand { get; }

        public Dictionary<string, string> Notification
        {
            get => notification;
            set
            {
                SetProperty(ref notification, value);
                NotificationMessage = value["message"];
                Description = value["description"];
            }
        }
        public string NotificationMessage { get => notificationMessage; set => SetProperty(ref notificationMessage, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }

        public SoSPopupViewModel()
        {
            NavigateToSoSListCommand = new AsyncCommand(ExecuteNavigateToSosListAsync);
            ClosePopViewCommand = new AsyncCommand(ExecuteClosePopViewAsync);
        }

        private async Task ExecuteNavigateToSosListAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//ReportListPage"));
        }

        private async Task ExecuteClosePopViewAsync()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync(true);
        }
    }
}
