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

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<Checkpoint> _genericCheckpointService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService;
        private bool showMap;
        private bool showLogo;
        private bool hasNoPatrol;
        private bool hasPatrol;
        private bool isStarted;
        private bool isNotStarted;
        private bool hasNextCheckpoint;
        private Patrol patrol;
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
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public PatrolCheckpoint PatrolCheckpoint { get => patrolCheckpoint; set => SetProperty(ref patrolCheckpoint, value); }
        public string NoPatrolMessage { get => noPatrolMessage; set => SetProperty(ref noPatrolMessage, value); }
        public Polyline Polyline { get => polyline; set => SetProperty(ref polyline, value); }

        public DashboardViewModel()
        {
            ShowMap = true;
            User = new ApplicationUser();
            Patrol = new Patrol();
            IsStarted = false;
            HasNoPatrol = true;
            _genericPatrolService = new GenericService<Patrol>();
            _genericCheckpointService = new GenericService<Checkpoint>();
            _genericUserService = new GenericService<ApplicationUser>();
            _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
            PinCollection = new ObservableRangeCollection<Pin>();
            ScanCommand = new AsyncCommand(ExecuteScanAsync);
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
            NavigateToPatrolListCommand = new AsyncCommand(ExecuteNavigateToPatrolListAsync);
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
            StartCommand = new AsyncCommand(ExecuteStart);
            EndCommand = new AsyncCommand(ExecuteEnd);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }
        private async Task ExecuteLogout()
        {
            try
            {
                IsErrorMessage = false;
                await LogoutService.LogoutAsync();
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
                string url = "patrols/" + Patrol.Id;
                ObjectResponse<Patrol> response = await _genericPatrolService.GetDetailsAsync(url, Token);
                Checkpoint checkpoint = null;
                if (response.Data.Route == null)
                {
                    return;
                }
                var checkpointResponse = response.Data.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                if (checkpointResponse != null)
                {
                    checkpoint = checkpointResponse.Checkpoint;
                }
                else
                {
                    checkpoint = response.Data.Route.RouteCheckpoints.FirstOrDefault().Checkpoint;
                }
                await NFCService.ExecuteScanAsync(checkpoint.Latitude, checkpoint.Longitude);
                if (!response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "All checkpoint scanned.", "OK", FlowDirection.RightToLeft);
                }
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
                IsErrorMessage = false;
                string url = "patrols/update-status";
                var content = new
                {
                    Id = Patrol.Id,
                    Status = "Started"
                };
                ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(url, content, Token);
                IsStarted = response.IsSuccess;
                if (IsStarted)
                {
                    var patrolResponse = await PatrolService.GetPatrolStatus();
                    if (patrolResponse.Data != null)
                    {
                        await PatrolDBService.Insert(patrolResponse.Data);
                        await TimeOutService.CheckTimerToken();
                    }
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", response.Message)))
                    );
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol Start Failed.", response.Message)))
                    );
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
                IsErrorMessage = false;
                string url = "patrols/update-status";
                var content = new
                {
                    Id = Patrol.Id,
                    Status = "Completed"
                };
                ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(url, content, Token);
                HasNoPatrol = !response.IsSuccess;
                if (response.IsSuccess)
                {
                    await TimeOutService.CheckTimerToken();
                    HasNoPatrol = true;
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol ended")))
                    );
                    await PatrolDBService.Delete();
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol notification", "Patrol End Failed", response.Message)))
                    );
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

        public void OnAppearing()
        {
            IsErrorMessage = IsNotConnected;
            Task.Run(async () => { await GetTokenAsync(); });
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
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
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
                Patrol = await PatrolDBService.Get();
                if (Patrol == null)
                {
                    ObjectResponse<Patrol> response = await PatrolService.GetPatrolStatus();
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol = response.Data;
                    }
                    else
                    {
                        ShowMap = false;
                        SetErrorMessage(response.Message, response.Errors);
                    }
                }
                
                if (Patrol != null)
                {
                    GeneratePinCollection();
                    if (Patrol.Status == "Scheduled" || Patrol.Status == "Started")
                    {
                        await TimeOutService.RunLocationBroadcastAsync(Token);
                        HasNoPatrol = false;
                        IsStarted = Patrol.Status == "Started";
                        await CheckNextPointAsync(Patrol);
                    }
                    else if (Patrol.Status == "Completed" || Patrol.Status == "Missed")
                    {
                        await PatrolDBService.Delete();
                        await TimerDBService.Delete();
                        ShowMap = false;
                        HasNoPatrol = true;
                        HasNextCheckpoint = false;
                        NoPatrolMessage = "Patrol complete for today.";
                    }
                }
                else
                {
                    ShowMap = false;
                    NoPatrolMessage = "No patrol for today.";
                }
            }
            catch (Exception)
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

        private async Task CheckNextPointAsync(Patrol patrol)
        {
            if (patrol.PatrolCheckpoints != null && patrol.PatrolCheckpoints.Count > 0)
            {
                PatrolCheckpoint = patrol.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                if (PatrolCheckpoint != null)
                {
                    if (PatrolCheckpoint.ExpectedCheckedTime != null)
                    {
                        TimeOutService.RunTimer();
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
    }
}
