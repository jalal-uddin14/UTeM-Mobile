using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.Views.Supervisor;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolListViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;

        public ICommand NavigateToPatrolAddCommand { get; }
        public ObservableRangeCollection<Patrol> PatrolList { get; set; }
        public PatrolListViewModel()
        {
            _genericService = new GenericService<Patrol>();
            PatrolList = new ObservableRangeCollection<Patrol>();
            NavigateToPatrolAddCommand = new AsyncCommand(ExecuteNavigateToPatrolAdd);
        }

        private async Task ExecuteNavigateToPatrolAdd()
        {
            await Shell.Current.GoToAsync($"{nameof(PatrolAddPage)}");
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetPatrolList(); });
        }

        private async Task GetPatrolList()
        {
            string url = "patrols";
            PaginatedResponse<Patrol> response = await _genericService.GetPagedListAsync(url);
            PatrolList.Clear();
            PatrolList.AddRange(response.Data.Data);
        }
    }
}
