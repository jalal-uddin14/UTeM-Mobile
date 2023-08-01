using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class GuardListViewModel : BaseViewModel
    {
        private IGenericService<ApplicationUser> _genericService;
        private AuthToken token;
        private bool isGuardVisible;
        private bool isRouteVisible;
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }
        public bool IsGuardVisible { get => isGuardVisible; set => SetProperty(ref isGuardVisible, value); }
        public bool IsRouteVisible { get => isRouteVisible; set => SetProperty(ref isRouteVisible, value); }

        public GuardListViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetGuardList();
            }
        }

        private async Task GetGuardList()
        {
            try
            {
                GuardList.Clear();
                string route = "guards";
                PaginatedResponse<ApplicationUser> response = await _genericService.GetPagedListAsync(route, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    GuardList.AddRange(response.Data.Data);
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Unexpected error occured" },
                        { "Message",  response.Message},
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
    }
}
