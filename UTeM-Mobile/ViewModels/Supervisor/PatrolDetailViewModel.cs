using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Services.DBServices;
using System.Windows.Input;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private readonly ITokenStorageService _tokenService;
        private string id;
        private Patrol patrol;
        private PatrolDetail patrolDetail;
        private Location location;
        private bool showMapRoute;
        private bool showGPS;
        public bool patrolRunning;

        public ICommand ToggleMapCommand { get; }

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }
        public Location Location { get => location; set => SetProperty(ref location, value); }
        public bool ShowMapRoute
        {
            get => showMapRoute;
            set
            {
                SetProperty(ref showMapRoute, value);
                ShowGPS = !value;
            }
        }
        public bool ShowGPS { get => showGPS; set => SetProperty(ref showGPS, value); }
        public bool PatrolRunning { get => patrolRunning; set => SetProperty(ref patrolRunning, value); }

        public PatrolDetailViewModel(ITokenStorageService tokenService)
        {
            Patrol = new Patrol { Guard = new ApplicationUser(), Route = new Route() };
            PatrolDetail = new PatrolDetail { Patrol = Patrol };
            _genericService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
            ToggleMapCommand = new Command(ExecuteToggleMap);
            _tokenService = tokenService;
        }

        private void ExecuteToggleMap()
        {
            ShowMapRoute = !ShowMapRoute;
        }

        public async Task OnAppearing()
        {
            PatrolRunning = false;
            IsBusy = true;
            ShowMapRoute = true;
            IsErrorMessage = false;
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (tokenJson != null)
                {
                    await GetPatrolDetailAsync();
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
                PatrolRunning = PatrolDetail != null && PatrolDetail.Status == "Started" && IsNotBusy;
            }
        }

        private async Task GetPatrolDetailAsync()
        {
            try
            {
                IsBusy = true;
                if (Id != null)
                {
                    string url = "patrolDetails/" + Id;
                    ObjectResponse<PatrolDetail> response = await _genericPatrolDetailService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        PatrolDetail = response.Data;
                        if (PatrolDetail.Patrol != null && PatrolDetail.Patrol.Route != null && PatrolDetail.Patrol.Route.RouteCheckpoints != null)
                        {
                            Patrol = PatrolDetail.Patrol;
                            var checkpoint = PatrolDetail.Patrol.Route.RouteCheckpoints.FirstOrDefault();
                            Location = new Location
                            {
                                Latitude = checkpoint.Checkpoint.Latitude,
                                Longitude = checkpoint.Checkpoint.Longitude,
                                Speed = 5
                            };
                        }
                        await CheckRouteCheckpointStatusAsync();
                    }
                    else
                    {
                        SetErrorMessage(response.Message, response.Errors);
                    }
                }
                else
                {
                    SetErrorMessage("Patrol not found.");
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CheckRouteCheckpointStatusAsync()
        {
            try
            {
                string url = "";
                for (int i = 0; i < Patrol.Route.RouteCheckpoints.Count; i++)
                {
                    Patrol.Route.RouteCheckpoints[i].IsNotLast = i < Patrol.Route.RouteCheckpoints.Count - 1;
                    url = "patrolCheckpoints/check";
                    var content = new { patrolDetailId = PatrolDetail.Id, checkpointId = Patrol.Route.RouteCheckpoints[i].CheckpointId };
                    ObjectResponse<PatrolCheckpoint> response = await _patrolCheckpointService.PostAsync(url, content, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol.Route.RouteCheckpoints[i].IsChecked = response.Data.Status == "Completed";
                        Patrol.Route.RouteCheckpoints[i].IsScheduled = response.Data.Status == "Scheduled";
                        Patrol.Route.RouteCheckpoints[i].IsMissed = response.Data.Status == "Missed";
                        Patrol.Route.RouteCheckpoints[i].NotFound = false;
                        if (response.Data.Status == "Scheduled")
                        {
                            Location = new Location { Latitude = Patrol.Route.RouteCheckpoints[i].Checkpoint.Latitude, Longitude = Patrol.Route.RouteCheckpoints[i].Checkpoint.Longitude, Speed = 5 };
                        }
                        Patrol.Route.RouteCheckpoints[i].NotFound = false;
                    }
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

    }
}
