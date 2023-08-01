using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.ViewModels.Guard
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailViewModel : BaseViewModel, IOnAppearing
    {
        private AuthToken token;
        private IGenericService<Patrol> _genericService;
        private IGenericService<PatrolCheckpoint> _patrolCheckpointService;
        private string id;
        private Patrol patrol;

        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }

        public PatrolDetailViewModel()
        {
            _genericService = new GenericService<Patrol>();
            _patrolCheckpointService = new GenericService<PatrolCheckpoint>();
            Patrol = new Patrol();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            token = await LocalDBService.GetToken();
            if (token != null)
            {
                await GetPatrolDetailAsync();
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
                    ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url, token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol = response.Data;
                        await CheckRouteCheckpointStatusAsync();
                    }
                    else
                    {
                        Dictionary<string, string> popupContent = new Dictionary<string, string>
                        {
                            { "Heading", "Error" },
                            { "Title", "Unexpected error occured" },
                            { "Message",  response.Message},
                            { "NavigateTo", "" },
                            { "HasNavigate", "" },
                        };
                        await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                    }
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Internal error occured" },
                        { "Message", "" },
                        { "NavigateTo", "" },
                        { "HasNavigate", "" },
                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                {
                    { "Heading", "Error" },
                    { "Title", "Server error occured" },
                    { "Message", "" },
                    { "NavigateTo", "" },
                    { "HasNavigate", "" },
                };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CheckRouteCheckpointStatusAsync()
        {
            string url = "";
            for (int i = 0; i < Patrol.Route.RouteCheckpoints.Count; i++)
            {
                Patrol.Route.RouteCheckpoints[i].IsNotLast = i < Patrol.Route.RouteCheckpoints.Count - 1;
                url = "patrolCheckpoints/check";
                var content = new { patrolId = Id, checkpointId = Patrol.Route.RouteCheckpoints[i].CheckpointId };
                ObjectResponse<PatrolCheckpoint> res = await _patrolCheckpointService.PostAsync(url, content, token);
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
