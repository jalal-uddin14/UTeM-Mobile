using MvvmHelpers.Commands;
using MvvmHelpers;
using System.Windows.Input;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.IServices;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class DashboardViewModel : BaseViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private AuthToken token;
        private bool isStarted;
        private bool isNotStarted;
        private Patrol patrol;
        public ICommand StartCommand { get; }
        public ICommand EndCommand { get; }
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

        public DashboardViewModel()
        {
            Patrol = new Patrol();
            IsStarted = false;
            _genericService = new GenericService<Patrol>();
            StartCommand = new AsyncCommand(ExecuteStart);
            EndCommand = new AsyncCommand(ExecuteEnd);
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
            IsStarted = false;
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
                await GetUserPatrol();
            }
        }

        private async Task GetUserPatrol()
        {
            string url = "patrols/status";
            ObjectResponse<Patrol> response = await _genericService.InsertAsync(url, null, token);
            Patrol = response.Data;
            IsStarted = Patrol.Status == "Started";
        }
    }
}
