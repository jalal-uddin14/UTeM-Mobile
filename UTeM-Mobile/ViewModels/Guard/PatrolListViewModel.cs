using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class PatrolListViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;

        public ObservableRangeCollection<Patrol> PatrolList { get; }

        public PatrolListViewModel()
        {
            _genericService = new GenericService<Patrol>();
            PatrolList = new ObservableRangeCollection<Patrol>();
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
