using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private string id;
        private Patrol patrol;
        private AuthToken token;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public PatrolDetailViewModel()
        {
            Patrol = new Patrol { Guard = new ApplicationUser(), Route = new Route() };
            _genericService = new GenericService<Patrol>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetPatrolDetailAsync();
            }
        }

        private async Task GetPatrolDetailAsync()
        {
            IsBusy = true;
            if (Id != null)
            {
                string url = "patrols/" + Id;
                ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url, Token);
                Patrol = response.Data;
                await CheckRouteCheckpointStatusAsync();
            }
            else
            {
                Console.WriteLine("Exception");
            }
            IsBusy = false;
        }

        private async Task CheckRouteCheckpointStatusAsync()
        {
            string url = "";
            for (int i = 0; i < Patrol.Route.RouteCheckpoints.Count; i++)
            {
                Patrol.Route.RouteCheckpoints[i].IsNotLast = i < Patrol.Route.RouteCheckpoints.Count - 1;
                url = "patrolCheckpoints/check";
                var content = new { patrolId = Id, checkpointId = Patrol.Route.RouteCheckpoints[i].CheckpointId };
                ObjectResponse<PatrolCheckpoint> res = await _patrolCheckpointService.InsertAsync(url, content);
                if (res.Data != null)
                {
                    Patrol.Route.RouteCheckpoints[i].IsChecked = res.Data.Status == "Completed";
                    Patrol.Route.RouteCheckpoints[i].IsScheduled = res.Data.Status == "Scheduled";
                    Patrol.Route.RouteCheckpoints[i].NotFound = false;
                }
            }
        }
    }
}
