using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolAddViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _userService;
        private IGenericService<Route> _routeService;
        private IGenericService<Patrol> _patrolService;
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
                if (SelectedGuard == null || SelectedGuard.Id == null)
                {
                    SetErrorMessage("Guard is required"); return;
                }
                if (SelectedRoute == null || SelectedRoute.Id == 0)
                {
                    SetErrorMessage("Route is required"); return;
                }
                IsErrorMessage = false;
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
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetNavigationMessage("Patrol", "Route assisg", "PatrolListPage", "Route successfully assigned.")))
                    );
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Error", "Route assign failed.", response.Message)))
                    );
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        public void OnAppearing()
        {
            IsGuardNotVisible = IsRouteNotVisible = true;
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
                    await GetGuardListAsync();
                    await GetRouteListAsync();
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetGuardListAsync()
        {
            try
            {
                IsGuardNotVisible = true;
                GuardList.Clear();
                string guardUrl = string.Format("guards?PageSize={0}", 100);
                PaginatedResponse<ApplicationUser> response = await _userService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    GuardList.AddRange(response.Data.Data);
                    IsGuardNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception ex)
            {
                GuardList.Clear();
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetRouteListAsync()
        {
            try
            {
                IsRouteNotVisible = true;
                RouteList.Clear();
                string guardUrl = "routes?PageSize=100";
                PaginatedResponse<Route> response = await _routeService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    RouteList.AddRange(response.Data.Data);
                    IsRouteNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }
    }
}
