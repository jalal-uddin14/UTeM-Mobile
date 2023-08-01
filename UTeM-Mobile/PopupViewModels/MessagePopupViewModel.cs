using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.PopupViewModels
{
    public class MessagePopupViewModel : MainViewModel
    {
        private Dictionary<string, string> content;
        private string heading;
        private string navigateTo;
        private bool showNavigate;
        private string buttonText;

        public ICommand NavigationCommand { get; }
        public ICommand ClosePopViewCommand { get; }

        public Dictionary<string, string> Content
        {
            get => content;
            set
            {
                SetProperty(ref content, value);
                if (value.ContainsKey("Type"))
                {
                    ButtonText = value["Type"] == "SoS" ? "SoS List" : "Patrol List";
                }
                Heading = value["Heading"];
                Title = value["Title"];
                Message = value["Message"];
                NavigateTo = "//" + value["NavigateTo"];
                ShowNavigate = value["HasNavigate"] == "true";
            }
        }

        public string Heading { get => heading; set => SetProperty(ref heading, value); }
        public string NavigateTo { get => navigateTo; set => SetProperty(ref navigateTo, value); }
        public bool ShowNavigate { get => showNavigate; set => SetProperty(ref showNavigate, value); }
        public string ButtonText { get => buttonText; set => SetProperty(ref buttonText, value); }

        public MessagePopupViewModel()
        {
            ButtonText = "Goto list";
            ClosePopViewCommand = new AsyncCommand(ExecuteClosePopViewAsync);
            NavigationCommand = new AsyncCommand(ExecuteNavigationAsync);
        }

        private async Task ExecuteNavigationAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(NavigateTo));
        }

        private async Task ExecuteClosePopViewAsync()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync(true);
        }
    }
}
