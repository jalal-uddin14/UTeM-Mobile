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
using System;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolAddViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _userService;
        private IGenericService<Campus> _campusService;
        private IGenericService<Route> _routeService;
        private IGenericService<Patrol> _patrolService;
        private IGenericService<Shift> _shiftService;
        private IGenericService<TimeSchedule> _timeService;
        private Patrol patrol;
        private ApplicationUser selectedGuard;
        private Campus selectedCampus;
        private Route selectedRoute;
        private Shift selectedShift;
        private TimeSchedule selectedTime;
        private bool isTimerEnable;

        private DateTime startDate;
        private DateTime endDate;

        private bool isGuardNotVisible;
        private bool isGuardVisible;

        private bool isCampusNotVisible;
        private bool isCampusVisible;

        private bool isRouteNotVisible;
        private bool isRouteVisible;
        private bool isRouteSelected;

        private bool isShiftNotVisible;
        private bool isShiftVisible;
        private bool isShiftSelected;

        private bool isTimeNotVisible;
        private bool isTimeVisible;
        private bool isTimeSelected;

        private string selectedRouteString;
        private string selectedShiftString;
        private string selectedTimeString;

        public ICommand CreatePatrolCommand { get; }
        public ICommand ReloadDataCommand { get; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public ObservableRangeCollection<Campus> CampusList { get; }
        public ObservableRangeCollection<Route> RouteList { get; }
        public ObservableRangeCollection<Shift> ShiftList { get; }
        public ObservableRangeCollection<TimeSchedule> TimeScheduleList { get; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public ApplicationUser SelectedGuard { get => selectedGuard; set => SetProperty(ref selectedGuard, value); }
        public Campus SelectedCampus
        {
            get => selectedCampus;
            set
            {
                SetProperty(ref selectedCampus, value);
                if (value != null && value.Id != 0)
                {
                    IsRouteSelected = false;
                    RouteList.Clear();
                    IsRouteNotVisible = true;
                    SelectedRouteString = "";
                    Task.Run(async () => await GetRouteListAsync());
                }
            }
        }
        public Route SelectedRoute
        {
            get => selectedRoute;
            set
            {
                SetProperty(ref selectedRoute, value);
                if (value != null && value.Id != 0)
                {
                    Task.Run(async() => await GetRouteDetail());
                }
            }
        }
        public Shift SelectedShift 
        { 
            get => selectedShift;
            set
            {
                SetProperty(ref selectedShift, value);
                if (value != null && value.Id != 0)
                {
                    Task.Run(async () => await GetShiftDetail());
                }
            }
        }
        public TimeSchedule SelectedTime 
        { 
            get => selectedTime;
            set
            {
                SetProperty(ref selectedTime, value);
                if (value != null && value.Id != 0)
                {
                    Task.Run(async () => await GetTimeDetail());
                }
            }
        }
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
        
        public bool IsCampusNotVisible
        {
            get => isCampusNotVisible;
            set
            {
                SetProperty(ref isCampusNotVisible, value);
                IsCampusVisible = !value;
            }
        }
        public bool IsCampusVisible { get => isCampusVisible; set => SetProperty(ref isCampusVisible, value); }

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
        public bool IsRouteSelected { get => isRouteSelected; set => SetProperty(ref isRouteSelected, value); }

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
        public bool IsShiftSelected { get => isShiftSelected; set => SetProperty(ref isShiftSelected, value); }

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
        public bool IsTimeSelected { get => isTimeSelected; set => SetProperty(ref isTimeSelected, value); }
        public string SelectedRouteString { get => selectedRouteString; set => SetProperty(ref selectedRouteString, value); }
        public string SelectedShiftString { get => selectedShiftString; set => SetProperty(ref selectedShiftString, value); }
        public string SelectedTimeString { get => selectedTimeString; set => SetProperty(ref selectedTimeString, value); }

        public PatrolAddViewModel()
        {
            Patrol = new Patrol();
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            SelectedGuard = new ApplicationUser();
            SelectedCampus = new Campus();
            SelectedRoute = new Route();
            SelectedShift = new Shift();
            SelectedTime = new TimeSchedule();
            CampusList = new ObservableRangeCollection<Campus>();
            RouteList = new ObservableRangeCollection<Route>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
            ShiftList = new ObservableRangeCollection<Shift>();
            TimeScheduleList = new ObservableRangeCollection<TimeSchedule>();
            CreatePatrolCommand = new AsyncCommand(ExecuteCreatePatrol);
            ReloadDataCommand = new MvvmHelpers.Commands.Command(OnAppearing);
            _userService = new GenericService<ApplicationUser>();
            _campusService = new GenericService<Campus>();
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
            IsRouteSelected = IsShiftSelected = IsTimeSelected = IsErrorMessage = false;
            SelectedRouteString = SelectedShiftString = SelectedTimeString = "";
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
                    await GetCampusListAsync();
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
                    MainThread.BeginInvokeOnMainThread(() => { GuardList.AddRange(response.Data.Data); });
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

        private async Task GetCampusListAsync()
        {
            try
            {
                IsCampusNotVisible = true;
                string campusUrl = "campuses?PageSize=100";
                PaginatedResponse<Campus> response = await _campusService.GetPagedListAsync(campusUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CampusList.Clear();
                        CampusList.AddRange(response.Data.Data);
                    });
                    IsCampusNotVisible = false;
                    await GetRouteListAsync();
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

        private async Task GetRouteListAsync()
        {
            try
            {
                IsRouteNotVisible = true;
                string guardUrl = "routes?PageSize=100&campusId=";
                if (SelectedCampus != null && SelectedCampus.Id != 0)
                {
                    guardUrl += SelectedCampus.Id.ToString();
                }
                
                PaginatedResponse<Route> response = await _routeService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RouteList.Clear();
                        RouteList.AddRange(response.Data.Data);
                    });
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

        private async Task GetShiftListAsync()
        {
            try
            {
                IsShiftNotVisible = true;
                string guardUrl = "shifts";
                PaginatedResponse<Shift> response = await _shiftService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ShiftList.Clear();
                        ShiftList.AddRange(response.Data.Data);
                    });
                    IsShiftNotVisible = false;
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

        private async Task GetTimeListAsync()
        {
            try
            {
                IsTimeNotVisible = true;
                string guardUrl = "timeschedules";
                PaginatedResponse<TimeSchedule> response = await _timeService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TimeScheduleList.Clear();
                        TimeScheduleList.AddRange(response.Data.Data);
                    });
                    IsTimeNotVisible = false;
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
    
        private async Task GetRouteDetail()
        {
            if (SelectedRoute != null && SelectedRoute.Id != 0)
            {
                try
                {
                    string url = string.Format("routes/{0}", SelectedRoute.Id);
                    var response = await _routeService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null && response.Data.RouteCheckpoints.Count > 0)
                    {
                        IsRouteSelected = true;
                        int counter = 0;
                        SelectedRouteString = "";
                        foreach (var item in response.Data.RouteCheckpoints)
                        {
                            SelectedRouteString += item.Checkpoint != null ? item.Checkpoint.Name : "";
                            if (counter++ < response.Data.RouteCheckpoints.Count-1)
                            {
                                SelectedRouteString += ", ";
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    IsRouteSelected = false;
                }
            }
        }

        private async Task GetShiftDetail()
        {
            if (SelectedShift != null && SelectedShift.Id != 0)
            {
                try
                {
                    string url = string.Format("shifts/{0}", SelectedShift.Id);
                    var response = await _shiftService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        IsShiftSelected = true;
                        SelectedShiftString = string.Format("{0}-{1}", DateTime.Today.Add(SelectedShift.Start).ToString("hh:mm tt"), DateTime.Today.Add(SelectedShift.End).ToString("hh:mm tt"));
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async Task GetTimeDetail()
        {
            if (SelectedTime != null && SelectedTime.Id != 0)
            {
                try
                {
                    string url = string.Format("timeschedules/{0}", SelectedTime.Id);
                    var response = await _timeService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null && response.Data.TimeScheduleDetails.Count > 0)
                    {
                        IsTimeSelected = true;
                        int counter = 0;
                        SelectedTimeString = "";
                        foreach (var item in response.Data.TimeScheduleDetails)
                        {
                            SelectedTimeString += DateTime.Today.Add(item.Time).ToString("hh:mm tt");
                            if (counter++ < response.Data.TimeScheduleDetails.Count - 1)
                            {
                                SelectedTimeString += ", ";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
