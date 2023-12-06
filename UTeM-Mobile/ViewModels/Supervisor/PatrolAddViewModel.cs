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
        private IGenericService<Shift> _shiftService;
        private IGenericService<TimeSchedule> _timeService;
        private Patrol patrol;
        private ApplicationUser selectedGuard;
        private Route selectedRoute;
        private Shift selectedShift;
        private TimeSchedule selectedTime;
        private bool isTimerEnable;

        private DateTime startDate;
        private DateTime endDate;

        private bool isGuardNotVisible;
        private bool isGuardVisible;

        private bool isRouteNotVisible;
        private bool isRouteVisible;

        private bool isShiftNotVisible;
        private bool isShiftVisible;

        private bool isTimeNotVisible;
        private bool isTimeVisible;

        public ICommand CreatePatrolCommand { get; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public ObservableRangeCollection<Route> RouteList { get; }
        public ObservableRangeCollection<Shift> ShiftList { get; }
        public ObservableRangeCollection<TimeSchedule> TimeScheduleList { get; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public Shift SelectedShift { get => selectedShift; set => SetProperty(ref selectedShift, value); }
        public TimeSchedule SelectedTime { get => selectedTime; set => SetProperty(ref selectedTime, value); }
        public ApplicationUser SelectedGuard { get => selectedGuard; set => SetProperty(ref selectedGuard, value); }
        public Route SelectedRoute { get => selectedRoute; set => SetProperty(ref selectedRoute, value); }
        public bool IsTimerEnable { get => isTimerEnable; set => SetProperty(ref isTimerEnable, value); }
        public DateTime StartDate { get => startDate; set => SetProperty(ref startDate, value); }
        public DateTime EndDate { get => endDate; set => SetProperty(ref endDate, value); }
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

        public bool IsShiftNotVisible
        {
            get => isShiftNotVisible;
            set
            {
                SetProperty(ref isShiftNotVisible, value);
                IsShiftVisible = !value;
            }
        }
        public bool IsShiftVisible { get => isShiftVisible; set => SetProperty(ref isShiftVisible, value); }

        public bool IsTimeNotVisible
        {
            get => isTimeNotVisible;
            set
            {
                SetProperty(ref isTimeNotVisible, value);
                IsTimeVisible = !value;
            }
        }
        public bool IsTimeVisible { get => isTimeVisible; set => SetProperty(ref isTimeVisible, value); }

        public PatrolAddViewModel()
        {
            Patrol = new Patrol();
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            SelectedGuard = new ApplicationUser();
            SelectedRoute = new Route();
            SelectedShift = new Shift();
            SelectedTime = new TimeSchedule();
            RouteList = new ObservableRangeCollection<Route>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
            ShiftList = new ObservableRangeCollection<Shift>();
            TimeScheduleList = new ObservableRangeCollection<TimeSchedule>();
            CreatePatrolCommand = new AsyncCommand(ExecuteCreatePatrol);
            _userService = new GenericService<ApplicationUser>();
            _routeService = new GenericService<Route>();
            _patrolService = new GenericService<Patrol>();
            _shiftService = new GenericService<Shift>();
            _timeService = new GenericService<TimeSchedule>();
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
                if (SelectedShift == null || SelectedShift.Id == 0)
                {
                    SetErrorMessage("Shift is required"); return;
                }
                if (SelectedTime == null || SelectedTime.Id == 0)
                {
                    SetErrorMessage("Time is required"); return;
                }
                IsErrorMessage = false;
                string createPatrollUrl = "patrols";
                string start = StartDate.ToString("yyyy-MM-dd ");
                string end = EndDate.ToString("yyyy-MM-dd ");
                var content = new
                {
                    GuardId = SelectedGuard.Id,
                    RouteId = SelectedRoute.Id,
                    ShiftId = SelectedShift.Id,
                    TimeScheduleId = SelectedTime.Id,
                    Start = start,
                    End = end,
                    TimerEnabled = IsTimerEnable
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
            catch (Exception)
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
                    await GetShiftListAsync();
                    await GetTimeListAsync();
                }
            }
            catch (Exception)
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
            catch(Exception)
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
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetShiftListAsync()
        {
            try
            {
                IsShiftNotVisible = true;
                ShiftList.Clear();
                string guardUrl = "shifts";
                PaginatedResponse<Shift> response = await _shiftService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    ShiftList.AddRange(response.Data.Data);
                    IsShiftNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetTimeListAsync()
        {
            try
            {
                IsTimeNotVisible = true;
                TimeScheduleList.Clear();
                string guardUrl = "timeschedules";
                PaginatedResponse<TimeSchedule> response = await _timeService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    TimeScheduleList.AddRange(response.Data.Data);
                    IsTimeNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }
    }
}
