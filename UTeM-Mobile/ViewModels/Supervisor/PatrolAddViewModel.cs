using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;

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
        private bool isGuardNotVisible;
        private bool isGuardVisible;
        private bool isRouteNotVisible;
        private bool isRouteVisible;

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
        public bool IsGuardNotVisible
        {
            get => isGuardNotVisible;
            set
            {
                SetProperty(ref isGuardNotVisible, value);
                IsGuardVisible = !value;
            }
        }
        public bool IsGuardVisible { get => isGuardVisible; set => SetProperty(ref isGuardVisible, value); }
        public bool IsRouteNotVisible
        {
            get => isRouteNotVisible;
            set
            {
                SetProperty(ref isRouteNotVisible, value);
                IsRouteVisible = !value;
            }
        }
        public bool IsRouteVisible { get => isRouteVisible; set => SetProperty(ref isRouteVisible, value); }

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
            try
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
                ObjectResponse<Patrol> response = await _patrolService.PostAsync(createPatrollUrl, content, Token);
                if (response.IsSuccess)
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Type", "Patrol" },
                        { "Heading", "Route assign" },
                        { "Title", "Route successfully assigned." },
                        { "Message", "" },
                        { "NavigateTo", "PatrolListPage" },
                        { "HasNavigate", "true" },
                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Route assign failed." },
                        { "Message", response.Message },
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
        }

        public void OnAppearing()
        {
            IsGuardNotVisible = IsRouteNotVisible = true;
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
            try
            {
                IsGuardNotVisible = true;
                GuardList.Clear();
                string guardUrl = "guards";
                PaginatedResponse<ApplicationUser> response = await _userService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    GuardList.AddRange(response.Data.Data);
                    IsGuardNotVisible = false;
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
            catch(Exception ex)
            {
                GuardList.Clear();
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
        }

        private async Task GetRouteListAsync()
        {
            try
            {
                IsRouteNotVisible = true;
                RouteList.Clear();
                string guardUrl = "routes";
                PaginatedResponse<Route> response = await _routeService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    RouteList.AddRange(response.Data.Data);
                    IsRouteNotVisible = false;
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
            catch(Exception ex)
            {
                RouteList.Clear();
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
        }
    }
}
