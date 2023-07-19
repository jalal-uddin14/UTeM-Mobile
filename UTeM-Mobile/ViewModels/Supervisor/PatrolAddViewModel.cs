using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolAddViewModel : BaseViewModel
    {
        private IGenericService<ApplicationUser> _userService;
        private IGenericService<Route> _routeService;
        private IGenericService<Patrol> _patrolService;
        private AuthToken token;
        private Patrol patrol;
        private ApplicationUser selectedGuard;
        private Route selectedRoute;
        private TimeSpan start;
        private TimeSpan end;

        public ICommand CreatePatrolCommand { get; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public ObservableRangeCollection<Route> RouteList { get; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public ApplicationUser SelectedGuard { get => selectedGuard; set => SetProperty(ref selectedGuard, value); }
        public Route SelectedRoute { get => selectedRoute; set => SetProperty(ref selectedRoute, value); }
        public TimeSpan Start { get => start; set => SetProperty(ref start, value); }
        public TimeSpan End { get => end; set => SetProperty(ref end, value); }

        public PatrolAddViewModel()
        {
            Patrol = new Patrol
            {
                Date = DateTime.Today,
                Start = new DateTime(),
                End = new DateTime()
            };
            Start = new TimeSpan();
            End = new TimeSpan();
            SelectedGuard = new ApplicationUser();
            SelectedRoute = new Route();
            RouteList = new ObservableRangeCollection<Route>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
            CreatePatrolCommand = new AsyncCommand(ExecuteCreatePatrol);
            _userService = new GenericService<ApplicationUser>();
            _routeService = new GenericService<Route>();
            _patrolService = new GenericService<Patrol>();
        }

        private async Task ExecuteCreatePatrol()
        {
            string createPatrollUrl = "patrols";
            var content = new
            {
                GuardId = SelectedGuard.Id,
                RouteId = SelectedRoute.Id,
                Date = Patrol.Date.ToString("yyyy-MM-dd"),
                Start = (Patrol.Date + Start).ToString("HH:mm:ss"),
                End = (Patrol.Date + End).ToString("HH:mm:ss"),
            };
            ObjectResponse<Patrol> response = await _patrolService.InsertAsync(createPatrollUrl, content, Token);
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
                await GetGuardListAsync();
                await GetRouteListAsync();
            }
        }

        private async Task GetGuardListAsync()
        {
            string guardUrl = "guards";
            PaginatedResponse<ApplicationUser> response = await _userService.GetPagedListAsync(guardUrl, Token);
            GuardList.Clear();
            GuardList.AddRange(response.Data.Data);
        }

        private async Task GetRouteListAsync()
        {
            string guardUrl = "routes";
            PaginatedResponse<Route> response = await _routeService.GetPagedListAsync(guardUrl, Token);
            RouteList.Clear();
            RouteList.AddRange(response.Data.Data);
        }
    }
}
