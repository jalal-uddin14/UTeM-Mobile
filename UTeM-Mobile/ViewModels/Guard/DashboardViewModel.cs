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

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<Checkpoint> _genericCheckpointService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService;
        private bool hasNoPatrol;
        private bool hasPatrol;
        private bool isStarted;
        private bool isNotStarted;
        private Patrol patrol;
        private ApplicationUser user;
        private string noPatrolMessage;

        public ICommand LogoutCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToPatrolListCommand { get; }
        public ICommand NavigateToSendSoSCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand EndCommand { get; }
        public bool HasNoPatrol
        {
            get => hasNoPatrol;
            set
            {
                SetProperty(ref hasNoPatrol, value);
                HasPatrol = !value;
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
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public string NoPatrolMessage { get => noPatrolMessage; set => SetProperty(ref noPatrolMessage, value); }

        public DashboardViewModel()
        {
            User = new ApplicationUser();
            Patrol = new Patrol();
            IsStarted = false;
            HasNoPatrol = true;
            _genericPatrolService = new GenericService<Patrol>();
            _genericCheckpointService = new GenericService<Checkpoint>();
            _genericUserService = new GenericService<ApplicationUser>();
            _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
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
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task ExecuteScanAsync()
        {
            try
            {
                string url = "patrols/" + Patrol.Id;
                string patrolCheckpointUrl = "patrolCheckpoints/mark";
                ObjectResponse<Patrol> response = await _genericPatrolService.GetDetailsAsync(url, Token);
                ObjectResponse<PatrolCheckpoint> objectResponse = null;
                if (response.Data.PatrolCheckpoints.Count <= 0)
                {
                    var content = new
                    {
                        patrolId = Patrol.Id,
                        checkpointId = response.Data.Route.RouteCheckpoints.FirstOrDefault().CheckpointId,
                        checkedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    objectResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, Token);
                    if (objectResponse.IsSuccess)
                    {
                        await App.Current.MainPage.DisplayAlert("Success", response.Data.Route.RouteCheckpoints.FirstOrDefault().Checkpoint.Name + " checkpoint successfully scaned.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Failed", response.Data.Route.RouteCheckpoints.FirstOrDefault().Checkpoint.Name + " checkpoint scan failed.", "OK");
                    }
                }
                else if (response.Data.PatrolCheckpoints.Count < response.Data.Route.RouteCheckpoints.Count)
                {
                    int index = 0;
                    var checkpoints = response.Data.Route.RouteCheckpoints;
                    for (int i = 0; i < checkpoints.Count; i++)
                    {
                        var isFound = response.Data.PatrolCheckpoints.Where(p => p.CheckpointId == checkpoints[i].CheckpointId).FirstOrDefault();
                        if (isFound == null)
                        {
                            index = i;
                            break;
                        }
                    }
                    var content = new
                    {
                        patrolId = Patrol.Id,
                        checkpointId = response.Data.Route.RouteCheckpoints[index].CheckpointId,
                        checkedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    objectResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, Token);
                    if (objectResponse.IsSuccess)
                    {
                        await App.Current.MainPage.DisplayAlert("Success", response.Data.Route.RouteCheckpoints.FirstOrDefault().Checkpoint.Name + " checkpoint successfully scaned.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Failed", response.Data.Route.RouteCheckpoints.FirstOrDefault().Checkpoint.Name + " checkpoint scan failed.", "OK");
                    }
                    if (index == checkpoints.Count - 1)
                    {
                        await ExecuteEnd();
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Success", "All checkpoint scanned.", "OK", FlowDirection.RightToLeft);
                }
            }
            catch(Exception ex)
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
                    Status = "Started",
                    StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(url, content, Token);
                IsStarted = response.IsSuccess;
                if (IsStarted)
                {
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
            catch(Exception ex )
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
                    Status = "Completed",
                    CompletedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(url, content, Token);
                IsStarted = !response.IsSuccess;
                if (response.IsSuccess)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification")))
                    );
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => 
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol notification", "Patrol End Failed", response.Message)))
                    );
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

        public void OnAppearing()
        {
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
                    await GetUserDetailAsync();
                    await GetUserPatrol();
                    await ShowMessageAsync();
                }
            }
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured, please try again.");
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
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured, please try again.");
            }
        }

        private async Task GetUserPatrol()
        {
            try
            {
                string url = "patrols/status";
                ObjectResponse<Patrol> response = await _genericPatrolService.PostAsync(url, null, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    if (response.Data.Status == "Scheduled" || response.Data.Status == "Started")
                    {
                        HasNoPatrol = false;
                        Patrol = response.Data;
                        IsStarted = Patrol.Status == "Started";
                    }
                    else if (response.Data.Status == "Completed" || response.Data.Status == "Missed")
                    {
                        NoPatrolMessage = "Patrol complete for today.";
                    }
                }
                else if (response.IsSuccess)
                {
                    NoPatrolMessage = "No patrol for today.";
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception ex)
            {
                HasNoPatrol = true;
                SetErrorMessage("Internal error occured.");
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
