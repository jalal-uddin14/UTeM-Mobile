using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class RouteDetailViewModel : BaseViewModel
    {
        private IGenericService<Route> genericService;
        private readonly ITokenStorageService _tokenService;
        private int id;
        private Route route;
        private AuthToken token;

        public ObservableRangeCollection<RouteCheckpoint> RouteCheckpointList { get; }
        public int Id { get => id; set => id = value; }
        public Route Route { get => route; set => SetProperty(ref route, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public RouteDetailViewModel(ITokenStorageService tokenService)
        {
            Route = new Route
            {
                RouteCheckpoints = new List<RouteCheckpoint>()
            };
            RouteCheckpointList = new ObservableRangeCollection<RouteCheckpoint>();
            genericService = new GenericService<Route>();
            _tokenService = tokenService;
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            var tokenJson = await _tokenService.GetAccessTokenAsync();
            if (tokenJson != null)
            {
                await GetRouteDetail();
            }
        }

        private async Task GetRouteDetail()
        {
            try
            {
                string url = "routes/" + Id;
                ObjectResponse<Route> response = await genericService.GetDetailsAsync(url);
                Route = response.Data;
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    RouteCheckpointList.Clear();
                    RouteCheckpointList.AddRange(Route.RouteCheckpoints);
                });
            }
            catch(Exception)
            {

            }
        }
    }
}
