using MvvmHelpers.Commands;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : BaseViewModel
    {
        public ICommand StartCommand { get; }

        public DashboardViewModel()
        {
            StartCommand = new AsyncCommand(ExecuteStart);
        }

        private async Task ExecuteStart()
        {

        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetUserPatrol(); });
        }

        private async Task GetUserPatrol()
        {

        }
    }
}
