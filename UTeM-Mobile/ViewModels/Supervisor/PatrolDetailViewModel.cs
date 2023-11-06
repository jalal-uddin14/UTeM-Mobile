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
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private string id;
        private Patrol patrol;
        private Location location;
        private bool showMapRoute;
        private bool showGPS;
        public bool patrolRunning;

        public ICommand ToggleMapCommand { get; }

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
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

        public PatrolDetailViewModel()
        {
            Patrol = new Patrol { Guard = new ApplicationUser(), Route = new Route() };
            _genericService = new GenericService<Patrol>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
            ToggleMapCommand = new Command(ExecuteToggleMap);
        }

        private void ExecuteToggleMap()
        {
            ShowMapRoute = !ShowMapRoute;
        }

        public void OnAppearing()
        {
            PatrolRunning = false;
            IsBusy = true;
            ShowMapRoute = true;
            IsErrorMessage = false;
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                Token = await LocalDBService.GetToken();
                if (Token != null)
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
                PatrolRunning = Patrol != null && Patrol.Status == "Started" && IsNotBusy;
            }
        }

        private async Task GetPatrolDetailAsync()
        {
            try
            {
                IsBusy = true;
                if (Id != null)
                {
                    string url = "patrols/" + Id;
                    ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol = response.Data;
                        if (Patrol.Route != null && Patrol.Route.RouteCheckpoints != null)
                        {
                            var checkpoint = Patrol.Route.RouteCheckpoints.FirstOrDefault();
                            Location = new Location
                            {
                                Latitude = checkpoint.Checkpoint.Latitude,
                                Longitude = checkpoint.Checkpoint.Longitude,
                                Speed = 5
                            };
                        }
                        //await LocateGuardAsync();
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
                    var content = new { patrolId = Id, checkpointId = Patrol.Route.RouteCheckpoints[i].CheckpointId };
                    ObjectResponse<PatrolCheckpoint> response = await _patrolCheckpointService.PostAsync(url, content, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol.Route.RouteCheckpoints[i].IsChecked = response.Data.Status == "Completed";
                        Patrol.Route.RouteCheckpoints[i].IsScheduled = response.Data.Status == "Scheduled";
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
