using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Views.Guard;
using UTeM_Mobile.PopupViews;

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
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
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
            catch (Exception ex)
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
                IsBusy = true;
                string url = "patrols";
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
