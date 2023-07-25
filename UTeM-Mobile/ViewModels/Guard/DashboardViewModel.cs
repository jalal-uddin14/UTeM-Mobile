using MvvmHelpers.Commands;
using MvvmHelpers;
using System.Windows.Input;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Views.Guard;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : BaseViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private AuthToken token;
        private bool hasNoPatrol;
        private bool hasPatrol;
        private bool isStarted;
        private bool isNotStarted;
        private Patrol patrol;
        private ApplicationUser user;

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
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
            NavigateToPatrolListCommand = new AsyncCommand(ExecuteNavigateToPatrolListAsync);
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
            StartCommand = new AsyncCommand(ExecuteStart);
            EndCommand = new AsyncCommand(ExecuteEnd);
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
            if (response.IsSuccess && response.Data != null)
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
