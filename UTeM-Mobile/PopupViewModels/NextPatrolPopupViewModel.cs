using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.PopupViewModels
{
    public class NextPatrolPopupViewModel : MainViewModel
    {
        private PatrolDetail patrolDetail;
        public ICommand ClosePopViewCommand { get; }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }

        public NextPatrolPopupViewModel()
        {
            ClosePopViewCommand = new AsyncCommand(ExecuteClosePopViewAsync);
        }

        private async Task ExecuteClosePopViewAsync()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
