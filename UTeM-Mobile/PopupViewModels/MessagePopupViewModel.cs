using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.PopupViewModels
{
    public class MessagePopupViewModel : MainViewModel
    {
        private PopMessage popMessage;
        private string buttonText;

        public ICommand NavigationCommand { get; }
        public ICommand ClosePopViewCommand { get; }

        public PopMessage PopMessage
        {
            get => popMessage;
            set
            {
                SetProperty(ref popMessage, value);
                if (!string.IsNullOrEmpty(value.Type))
                {
                    ButtonText = value.Type == "SoS" ? "SoS List" : "Patrol List";
                }
            }
        }
        public string ButtonText { get => buttonText; set => SetProperty(ref buttonText, value); }

        public MessagePopupViewModel()
        {
            ButtonText = "Goto list";
            ClosePopViewCommand = new AsyncCommand(ExecuteClosePopViewAsync);
            NavigationCommand = new AsyncCommand(ExecuteNavigationAsync);
        }

        private async Task ExecuteNavigationAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//" + PopMessage.NavigateTo));
        }

        private async Task ExecuteClosePopViewAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
