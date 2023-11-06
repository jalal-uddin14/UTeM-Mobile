using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class GuardListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericService;
        private bool isGuardVisible;
        private bool isRouteVisible;
        public ObservableRangeCollection<ApplicationUser> GuardList { get; }
        public bool IsGuardVisible { get => isGuardVisible; set => SetProperty(ref isGuardVisible, value); }
        public bool IsRouteVisible { get => isRouteVisible; set => SetProperty(ref isRouteVisible, value); }

        public GuardListViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
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
                    await GetGuardList();
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured");
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
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured");
            }
        }
    }
}
