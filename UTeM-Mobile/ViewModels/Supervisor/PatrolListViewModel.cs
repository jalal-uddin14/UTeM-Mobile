using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Views.Supervisor;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private AuthToken token;
        private ApplicationUser user;

        public ICommand LogoutCommand { get; }
        public ICommand NavigateToGuardListCommand { get; set; }
        public ICommand NavigateToPatrolAddCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ObservableRangeCollection<Patrol> PatrolList { get; set; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public PatrolListViewModel()
        {
            User = new ApplicationUser();
            _genericService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
            PatrolList = new ObservableRangeCollection<Patrol>();
            NavigateToGuardListCommand = new AsyncCommand(ExecuteNavigateToGuardList);
            NavigateToPatrolAddCommand = new AsyncCommand(ExecuteNavigateToPatrolAdd);
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
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
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }
        private async Task ExecuteNavigateToGuardList()
        {
            await Shell.Current.GoToAsync($"{nameof(GuardListPage)}");
        }

        private async Task ExecuteNavigateToPatrolAdd()
        {
            await Shell.Current.GoToAsync($"{nameof(PatrolAddPage)}");
        }

        private async Task ExecuteNavigateToProfile()
        {
            await Shell.Current.GoToAsync($"{nameof(ProfilePage)}");
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
                IsBusy = true;
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
                    await GetProfileAsync();
                    await GetPatrolList();
                }
            }
            catch(Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
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
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetPatrolList()
        {
            try
            {
                PatrolList.Clear();
                string url = "patrols?date=" + DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");
                PaginatedResponse<Patrol> response = await _genericService.GetPagedListAsync(url, token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    PatrolList.AddRange(response.Data.Data);
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception ex)
            {
                PatrolList.Clear();
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
