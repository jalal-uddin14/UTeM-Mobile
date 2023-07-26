using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolAddViewModel : BaseViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _userService;
        private IGenericService<Route> _routeService;
        private IGenericService<Patrol> _patrolService;
        private AuthToken token;
        private Patrol patrol;
        private ApplicationUser selectedGuard;
        private Route selectedRoute;
        private DateTime startDate;
        private TimeSpan startTime;
        private DateTime endDate;
        private TimeSpan endTime;

        public ICommand CreatePatrolCommand { get; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public ObservableRangeCollection<Route> RouteList { get; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public ApplicationUser SelectedGuard { get => selectedGuard; set => SetProperty(ref selectedGuard, value); }
        public Route SelectedRoute { get => selectedRoute; set => SetProperty(ref selectedRoute, value); }
        public DateTime StartDate { get => startDate; set => SetProperty(ref startDate, value); }
        public TimeSpan StartTime { get => startTime; set => SetProperty(ref startTime, value); }
        public DateTime EndDate { get => endDate; set => SetProperty(ref endDate, value); }
        public TimeSpan EndTime { get => endTime; set => SetProperty(ref endTime, value); }

        public PatrolAddViewModel()
        {
            Patrol = new Patrol();
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            StartTime = new TimeSpan();
            EndTime = new TimeSpan();
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
            string start = StartDate.ToString("yyyy-MM-dd ") + new DateTime(StartTime.Ticks).ToString("HH:mm:ss");
            string end = EndDate.ToString("yyyy-MM-dd ") + new DateTime(EndTime.Ticks).ToString("HH:mm:ss");
            var content = new
            {
                GuardId = SelectedGuard.Id,
                RouteId = SelectedRoute.Id,
                Start = start,
                End = end,
            };
            ObjectResponse<Patrol> response = await _patrolService.InsertAsync(createPatrollUrl, content, Token);
            if (response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Success", "Route successfully assigned.", "OK");
                await Shell.Current.GoToAsync("//PatrolListPage");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Failed", "Route assign failed.", "OK");
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
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
