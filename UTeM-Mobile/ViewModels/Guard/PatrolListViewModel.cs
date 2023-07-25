using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Models;
using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class PatrolListViewModel : BaseViewModel, IOnAppearing
    {
        private AuthToken token;
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;

        public ICommand NavigateToSendSoSCommand { get; }
        public ObservableRangeCollection<Patrol> PatrolList { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public PatrolListViewModel()
        {
            _genericService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
            PatrolList = new ObservableRangeCollection<Patrol>();
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
        }
        private async Task ExecuteNavigateToSendSoSAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(ReportSendPage)}");
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
                await GetPatrolList();
                await GetUserDetailAsync();
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

        private async Task GetPatrolList()
        {
            try
            {
                IsBusy = true;
                string url = "patrols";
                PaginatedResponse<Patrol> response = await _genericService.GetPagedListAsync(url, token);
                PatrolList.Clear();
                PatrolList.AddRange(response.Data.Data);
            }
            catch(Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
