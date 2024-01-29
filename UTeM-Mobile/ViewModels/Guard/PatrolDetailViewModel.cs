using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Guard
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private string id;
        private Patrol patrol;
        private PatrolDetail patrolDetail;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }

        public PatrolDetailViewModel()
        {
            _genericPatrolService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
            Patrol = new Patrol();
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
            catch(Exception)
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
                    string url = "patrolDetails/" + Id;
                    ObjectResponse<PatrolDetail> response = await _genericPatrolDetailService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        PatrolDetail = response.Data;
                        if (PatrolDetail.Patrol != null)
                        {
                            Patrol = PatrolDetail.Patrol;
                            await CheckRouteCheckpointStatusAsync();
                        }
                    }
                    else
                    {
                        IsErrorMessage = false;
                        SetErrorMessage(response.Message, response.Errors);
                    }
                }
                else
                {
                    SetErrorMessage("Server error occured.");
                }
            }
            catch (Exception)
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
                    ObjectResponse<PatrolCheckpoint> res = await _patrolCheckpointService.PostAsync(url, content, Token);
                    if (res.IsSuccess && res.Data != null)
                    {
                        Patrol.Route.RouteCheckpoints[i].IsChecked = res.Data.Status == "Completed";
                        Patrol.Route.RouteCheckpoints[i].IsScheduled = res.Data.Status == "Scheduled";
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
