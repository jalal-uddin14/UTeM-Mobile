using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Views.Supervisor;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolListViewModel : BaseViewModel
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private AuthToken token;
        private ApplicationUser user;


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
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            IsBusy = true;
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetProfileAsync();
                await GetPatrolList();
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
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Internal error occured" },
                        { "Message", response.Message },
                        { "NavigateTo", "" },
                        { "HasNavigate", "" },
                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch(Exception ex)
            {
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                {
                    { "Heading", "Error" },
                    { "Title", "Server error occured" },
                    { "Message", "" },
                    { "NavigateTo", "" },
                    { "HasNavigate", "" },
                };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
            }
        }

        private async Task GetPatrolList()
        {
            try
            {
                PatrolList.Clear();
                string url = "patrols?date=" + DateTime.Now.Date;
                PaginatedResponse<Patrol> response = await _genericService.GetPagedListAsync(url, token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    PatrolList.AddRange(response.Data.Data);
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Internal error occured" },
                        { "Message", response.Message },
                        { "NavigateTo", "" },
                        { "HasNavigate", "" },
                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch(Exception ex)
            {
                PatrolList.Clear();
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                {
                    { "Heading", "Error" },
                    { "Title", "Server error occured" },
                    { "Message", "" },
                    { "NavigateTo", "" },
                    { "HasNavigate", "" },
                };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
