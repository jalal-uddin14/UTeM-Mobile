using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Views.Guard;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.StaticProperties;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using Plugin.LocalNotification;
using MvvmHelpers;
using Microsoft.Maui.Controls.Maps;
using UTeM_Mobile.Core.Services.DBServices;
using System.Text.Json;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : MainViewModel, IOnAppearing
    {
        private IDispatcherTimer timer = null;
        private Location currentLocation;

        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<ApplicationUser> _genericUserService;

        private readonly ITokenStorageService _tokenService;
        private readonly ILogoutService _logoutService;
        private readonly IPatrolService _patrolService;
        private readonly INFCService _nfcService;
        private readonly ITimeOutService _timeoutService;

        private bool showMap;
        private bool showLogo;
        private bool hasNoPatrol;
        private bool hasPatrol;
        private bool isStarted;
        private bool isNotStarted;
        private bool hasNextCheckpoint;
        private Patrol patrol;
        private PatrolDetail patrolDetail;
        private ApplicationUser user;
        private PatrolCheckpoint patrolCheckpoint;
        private string noPatrolMessage;
        private Polyline polyline;

        public ICommand LogoutCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToPatrolListCommand { get; }
        public ICommand NavigateToSendSoSCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand EndCommand { get; }

        public ObservableRangeCollection<Pin> PinCollection { get; }


        public Location CurrentLocation { get => currentLocation; set => SetProperty(ref currentLocation, value); }
        public bool ShowMap
        {
            get => showMap;
            set
            {
                SetProperty(ref showMap, value);
                ShowLogo = !value;
            }
        }
        public bool ShowLogo { get => showLogo; set => SetProperty(ref showLogo, value); }
        public bool HasNoPatrol
        {
            get => hasNoPatrol;
            set
            {
                SetProperty(ref hasNoPatrol, value);
                HasPatrol = !value;
                HasNextCheckpoint = !value;
            }
        }
        public bool HasPatrol { get => hasPatrol; set => SetProperty(ref hasPatrol, value); }
        public bool IsStarted
        {
            get => isStarted;
            set
            {
                SetProperty(ref isStarted, value);
                IsNotStarted = !value;
            }
        }
        public bool IsNotStarted { get => isNotStarted; set => SetProperty(ref isNotStarted, value); }
        public bool HasNextCheckpoint { get => hasNextCheckpoint; set => SetProperty(ref hasNextCheckpoint, value); }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public PatrolCheckpoint PatrolCheckpoint { get => patrolCheckpoint; set => SetProperty(ref patrolCheckpoint, value); }
        public string NoPatrolMessage { get => noPatrolMessage; set => SetProperty(ref noPatrolMessage, value); }
        public Polyline Polyline { get => polyline; set => SetProperty(ref polyline, value); }

        public DashboardViewModel(ILogoutService logoutService, ITokenStorageService tokenService, IPatrolService patrolService, INFCService nfcService, ITimeOutService timeOutService)
        {
            ShowMap = true;
            User = new ApplicationUser();
            Patrol = new Patrol();
            IsStarted = false;
            HasNoPatrol = true;

            _logoutService = logoutService;
            _tokenService = tokenService;
            _patrolService = patrolService;
            _nfcService = nfcService;
            _timeoutService = timeOutService;


            _genericPatrolService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _genericUserService = new GenericService<ApplicationUser>();
            PinCollection = new ObservableRangeCollection<Pin>();
            ScanCommand = new AsyncCommand(ExecuteScanAsync);
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
            NavigateToPatrolListCommand = new AsyncCommand(ExecuteNavigateToPatrolListAsync);
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
            StartCommand = new AsyncCommand(ExecuteStart);
            EndCommand = new AsyncCommand(ExecuteEnd);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
            _nfcService = nfcService;
        }
        private async Task ExecuteLogout()
        {
            try
            {
                IsErrorMessage = false;
                await _logoutService.LogoutAsync();
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task ExecuteScanAsync()
        {
            try
            {
                Checkpoint checkpoint = PatrolCheckpoint.Checkpoint;
                await _nfcService.ExecuteScanAsync(checkpoint);
            }
            catch(Exception)
            {

            }
        }

        private async Task ExecuteNavigateToProfile()
        {
            await Shell.Current.GoToAsync("GuardProfilePage");
        }

        private async Task ExecuteNavigateToPatrolListAsync()
        {
            await Shell.Current.GoToAsync("GuardPatrolListPage");
        }
        private async Task ExecuteNavigateToSendSoSAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(ReportSendPage)}");
        }

        private async Task ExecuteStart()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol starting")))
                );
                IsErrorMessage = false;
                string url = "patrolDetails/update-status";
                var content = new
                {
                    Id = PatrolDetail.Id,
                    Status = "Started"
                };
                ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(url, content, Token);
                IsStarted = response.IsSuccess;
                if (IsStarted)
                {
                    var patrolResponse = await _patrolService.GetPatrolStatus();
                    if (patrolResponse.Data != null)
                    {
                        StaticCredentials.PatrolDetail = patrolDetail;
                        await _timeoutService.CheckTimerToken();
                    }
                    await MainThread.InvokeOnMainThreadAsync(() => {
                        Application.Current.MainPage.Navigation.PopToRootAsync();
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", response.Message)));
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => {
                        Application.Current.MainPage.Navigation.PopToRootAsync();
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Information", "Patrol Notification.", response.Message)));
                    });
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task ExecuteEnd()
        {
            try
            {

                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol starting")))
                );
                IsErrorMessage = false;
                string url = "patrolDetails/update-status";
                var content = new
                {
                    Id = PatrolDetail.Id,
                    Status = "Completed"
                };
                ObjectResponse<PatrolDetail> response = await _genericPatrolDetailService.PutAsync(url, content, Token);
                HasNoPatrol = !response.IsSuccess;
                if (response.IsSuccess)
                {
                    await _timeoutService.CheckTimerToken();
                    StaticCredentials.PatrolDetail = null;
                    await PatrolDBService.Delete();
                    HasNoPatrol = true;
                    await MainThread.InvokeOnMainThreadAsync(() => {
                        Application.Current.MainPage.Navigation.PopToRootAsync();
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol ended")));
                    });
                    await PatrolDBService.Delete();
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => {
                        Application.Current.MainPage.Navigation.PopToRootAsync();
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol notification", "Patrol End Failed", response.Message)));
                    });
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }

        public async Task OnAppearing()
        {
            timer = Application.Current.Dispatcher.CreateTimer();
            RunTimer();
            IsErrorMessage = IsNotConnected;
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            try
            {
                if (IsNotConnected)
                {
                    return;
                }
                IsBusy = true;
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                Token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
                if (Token != null)
                {
                    await _timeoutService.CheckTimerToken();
                    await GetUserDetailAsync();
                    await GetUserPatrol();
                    await ShowMessageAsync();
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured, please try again.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetUserDetailAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response != null)
                {
                    User = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured, please try again.");
            }
        }

        private async Task GetUserPatrol()
        {
            try
            {
                PatrolDetail = StaticCredentials.PatrolDetail;
                if (PatrolDetail == null)
                {
                    ObjectResponse<PatrolDetail> response = await _patrolService.GetPatrolStatus();
                    if (response.IsSuccess && response.Data != null)
                    {
                        PatrolDetail = response.Data;
                        Patrol = PatrolDetail?.Patrol;
                    }
                }
                if (PatrolDetail != null)
                {
                    HasNoPatrol = false;
                    ShowMap = true;
                    ObjectResponse<Patrol> objectResponse = await _genericPatrolService.GetDetailsAsync(string.Format("patrols/{0}", PatrolDetail.PatrolId), Token);
                    if (objectResponse.IsSuccess && objectResponse.Data != null)
                    {
                        Patrol = objectResponse.Data;
                        if (Patrol.End < DateTime.UtcNow.AddHours(7) && PatrolDetail.Status == "Started")
                        {
                            //await ExecuteEnd();
                        }
                    }
                    if (PatrolDetail.Status == "Scheduled" || PatrolDetail.Status == "Started")
                    {
                        IsStarted = PatrolDetail.Status == "Started";
                        if (PatrolDetail.Status == "Scheduled")
                        {
                            HasNextCheckpoint = false;
                            ShowMap = false;
                            var a = DateTime.UtcNow.AddHours(8).AddMinutes(30);
                            if (PatrolDetail.Start > DateTime.UtcNow.AddHours(8).AddMinutes(30))
                            {
                                HasNoPatrol = true;
                                NoPatrolMessage = string.Format("Next patrol on {0}, at {1}", PatrolDetail.Start.ToString("ddd dd-MMM"), PatrolDetail.Start.ToString("hh:mm tt"));
                            }
                        }
                        else
                        {
                            GeneratePinCollection();
                            await _timeoutService.RunLocationBroadcastAsync(Token);
                            ObjectResponse<PatrolDetail> objectResponse1 = await _genericPatrolDetailService.GetDetailsAsync(string.Format("patrolDetails/{0}", PatrolDetail.Id), Token);
                            if (objectResponse1.IsSuccess && objectResponse1.Data != null)
                            {
                                await CheckNextPointAsync(objectResponse1.Data);
                            }
                        }
                    }
                    else if (PatrolDetail.Status == "Completed" || PatrolDetail.Status == "Missed")
                    {
                        StaticCredentials.CheckpointTimer = null;
                        ShowMap = false;
                        HasNoPatrol = true;
                        HasNextCheckpoint = false;
                        NoPatrolMessage = "Patrol complete for today.";
                    }
                }
                else
                {
                    ShowMap = false;
                    HasNoPatrol = true;
                    NoPatrolMessage = "You have no immediate patrol.";
                }
            }
            catch (Exception ex)
            {
                HasNoPatrol = true;
                SetErrorMessage("Internal error occured.");
            }
        }

        private void GeneratePinCollection()
        {
            if (Patrol.Route.RouteCheckpoints != null)
            {
                try
                {
                    List<Pin> pins = new List<Pin>();
                    foreach (var routeCheckpoint in Patrol.Route.RouteCheckpoints)
                    {
                        var Checkpoint = routeCheckpoint.Checkpoint;
                        var pin = new Pin
                        {
                            Label = Checkpoint.Name,
                            Location = new Location(Checkpoint.Latitude, Checkpoint.Longitude)
                        };
                        pins.Add(pin);
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        PinCollection.Clear();
                        PinCollection.AddRange(pins);
                    });
                }
                catch(Exception)
                {
                    SetErrorMessage("Failed to generate pins on map.");
                }
            }
        }

        private async Task CheckNextPointAsync(PatrolDetail patrolDetail)
        {
            if (patrolDetail.PatrolCheckpoints != null && patrolDetail.PatrolCheckpoints.Count > 0)
            {
                PatrolCheckpoint = patrolDetail.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                if (PatrolCheckpoint != null)
                {
                    if (PatrolCheckpoint.ExpectedCheckedTime != null)
                    {
                        if (Token.UserRole == "Guard")
                        {
                            if (patrolDetail.Patrol.TimerEnabled)
                            {
                                if (User != null)
                                {
                                    TimeOutService.RunPatrolDetailTimer(Token, User);
                                }
                                TimeOutService.RunPatrolTimer();
                                TimeOutService.RunShiftTimer();
                            }
                        }
                        var notification = new NotificationRequest
                        {
                            NotificationId = 100,
                            Title = "Patrol Notification",
                            Description = $"You have a checkpoint at {PatrolCheckpoint.ExpectedCheckedTime.Value.ToString("hh:mm tt")}",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = PatrolCheckpoint.ExpectedCheckedTime.Value
                            },
                            Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                            {
                                VisibilityType = Plugin.LocalNotification.AndroidOption.AndroidVisibilityType.Public,
                            }
                        };
                        PatrolCheckpoint.ExpectedCheckedTime = PatrolCheckpoint.ExpectedCheckedTime.Value;
                        await LocalNotificationCenter.Current.Show(notification);
                    }
                }
                HasNextCheckpoint = PatrolCheckpoint != null;
            }
        }

        private async Task ShowMessageAsync()
        {

            if (StaticMessage.HasNFCMessage)
            {
                await MainThread.InvokeOnMainThreadAsync(() => App.Current.MainPage.DisplayAlert("Warning", StaticMessage.NFCMessage, "Ok"));
                StaticMessage.NFCMessage = null;
                StaticMessage.HasNFCMessage = false;
            }
        }

        private void RunTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += async (s, e) =>
            {
                try
                {
                    CurrentLocation = await LocationService.GetCurrentLocationAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            timer.Start();
        }
    }
}
