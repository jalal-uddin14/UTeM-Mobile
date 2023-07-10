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
        private Patrol patrol;
        private ApplicationUser selectedGuard;
        private Route selectedRoute;

        public ICommand CreatePatrolCommand { get; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public ObservableRangeCollection<Route> RouteList { get; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public ApplicationUser SelectedGuard { get => selectedGuard; set => SetProperty(ref selectedGuard, value); }
        public Route SelectedRoute { get => selectedRoute; set => SetProperty(ref selectedRoute, value); }

        public PatrolAddViewModel()
        {
            Patrol = new Patrol
            {
                Date = DateTime.Now
            };
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
            patrol.GuardId = SelectedGuard.Id;
            patrol.RouteId = SelectedRoute.Id;
            ObjectResponse<Patrol> response = await _patrolService.InsertAsync(createPatrollUrl, Patrol);
            var a = response.Data;
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetGuardListAsync(); });
            Task.Run(async () => { await GetRouteListAsync(); });
        }

        private async Task GetGuardListAsync()
        {
            string guardUrl = "guards";
            PaginatedResponse<ApplicationUser> response = await _userService.GetPagedListAsync(guardUrl);
            GuardList.Clear();
            GuardList.AddRange(response.Data.Data);
        }

        private async Task GetRouteListAsync()
        {
            string guardUrl = "routes";
            PaginatedResponse<Route> response = await _routeService.GetPagedListAsync(guardUrl);
            RouteList.Clear();
            RouteList.AddRange(response.Data.Data);
        }
    }
}
