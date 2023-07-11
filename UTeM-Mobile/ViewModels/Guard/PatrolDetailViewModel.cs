using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;
        private string id;
        private Patrol patrol;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }

        public PatrolDetailViewModel()
        {
            _genericService = new GenericService<Patrol>();
            Patrol = new Patrol();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetPatrolDetailAsync(); });
        }

        private async Task GetPatrolDetailAsync()
        {
            string url = "patrols/" + Id;
            ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url);
            Patrol = response.Data;
        }
    }
}
