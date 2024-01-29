using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.PopupViewModels
{
    public class TimeoutPopupViewModel : MainViewModel
    {
        private PatrolCheckpoint _patrolCheckpoint;
        private CheckpointTimer _checkpointTimer;
        private PopMessage popMessage;

        public ICommand NavigationCommand { get; }
        public ICommand ClosePopViewCommand { get; }

        public PatrolCheckpoint PatrolCheckpoint { get => _patrolCheckpoint; set => SetProperty(ref _patrolCheckpoint, value); }
        public CheckpointTimer CheckpointTimer { get => _checkpointTimer; set => SetProperty(ref _checkpointTimer, value); }
        public PopMessage PopMessage
        {
            get => popMessage;
            set => SetProperty(ref popMessage, value);
        }

        public TimeoutPopupViewModel()
        {
            ClosePopViewCommand = new AsyncCommand(ExecuteClosePopViewAsync);
            NavigationCommand = new AsyncCommand(ExecuteNavigationAsync);
        }

        private async Task ExecuteNavigationAsync()
        {
            StaticCredentials.CheckpointTimer = null;
            await ModalService.PopAllModals();
            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//ReportSendPage"));
        }

        private async Task ExecuteClosePopViewAsync()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
