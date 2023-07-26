using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Views.Guard;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService;
        private bool hasNoPatrol;
        private bool hasPatrol;
        private bool isStarted;
        private bool isNotStarted;
        private Patrol patrol;
        private ApplicationUser user;

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

        public DashboardViewModel()
        {
            Patrol = new Patrol();
            IsStarted = false;
            _genericService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
            _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
            ScanCommand = new AsyncCommand(ExecuteScanAsync);
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
            NavigateToPatrolListCommand = new AsyncCommand(ExecuteNavigateToPatrolListAsync);
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
            StartCommand = new AsyncCommand(ExecuteStart);
            EndCommand = new AsyncCommand(ExecuteEnd);
        }

        private async Task ExecuteScanAsync()
        {
            string url = "patrols/" + Patrol.Id;
            string patrolCheckpointUrl = "patrolCheckpoints/mark";
            ObjectResponse<Patrol> response = await _genericService.GetDetailsAsync(url, token);
            ObjectResponse<PatrolCheckpoint> objectResponse = null;
            if (response.Data.PatrolCheckpoints.Count <= 0)
            {
                var content = new
                {
                    patrolId = Patrol.Id,
                    checkpointId = response.Data.Route.RouteCheckpoints.FirstOrDefault().CheckpointId,
                    checkedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                objectResponse = await _genericPatrolCheckpointService.InsertAsync(patrolCheckpointUrl, content, token);
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
                objectResponse = await _genericPatrolCheckpointService.InsertAsync(patrolCheckpointUrl, content, token);
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
            string url = "patrols/update-status";
            var content = new
            {
                Id = Patrol.Id,
                Status = "Started",
                StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            ObjectResponse<Patrol> response = await _genericService.UpdateAsync(url, content, token);
            IsStarted = response.IsSuccess;
            if (response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Success", "Patrol started.", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Failed", "Patrol start failed.", "OK");
            }
        }

        private async Task ExecuteEnd()
        {
            string url = "patrols/update-status";
            var content = new
            {
                Id = Patrol.Id,
                Status = "Completed",
                StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            ObjectResponse<Patrol> response = await _genericService.UpdateAsync(url, content, token);
            IsStarted = !response.IsSuccess;
            if (response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Success", "Patrol ended.", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Failed", "Patrol end failed.", "OK");
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            token = await LocalDBService.GetToken();
            if (token != null)
            {
                await GetUserDetailAsync();
                await GetUserPatrol();
            }
        }

        private async Task GetUserDetailAsync()
        {
            string url = "accounts/me";
            ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
            if (response != null)
            {
                User = response.Data;
            }
        }

        private async Task GetUserPatrol()
        {
            string url = "patrols/status";
            ObjectResponse<Patrol> response = await _genericService.InsertAsync(url, null, token);
            if (response.IsSuccess && response.Data != null && response.Data.Status != "Completed")
            {
                HasNoPatrol = false;
                Patrol = response.Data;
                IsStarted = Patrol.Status == "Started";
            }
            else
            {
                HasNoPatrol = true;
            }
        }
    }
}
