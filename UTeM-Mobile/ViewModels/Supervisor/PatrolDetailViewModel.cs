using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private string id;
        private Patrol patrol;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }

        public PatrolDetailViewModel()
        {
            Patrol = new Patrol { Guard = new ApplicationUser(), Route = new Route() };
            _genericService = new GenericService<Patrol>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
        }

        public void OnAppearing()
        {
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
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured.");
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
            catch(Exception ex)
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
                        Patrol.Route.RouteCheckpoints[i].NotFound = false;
                    }
                }
            }
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }
    }
}
