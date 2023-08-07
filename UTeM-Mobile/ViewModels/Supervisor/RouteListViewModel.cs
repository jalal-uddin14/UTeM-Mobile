using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class RouteListViewModel : BaseViewModel
    {
        private IGenericService<Route> _genericService;
        private AuthToken token;
        public ObservableRangeCollection<Route> RouteList { get; }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public RouteListViewModel()
        {
            _genericService = new GenericService<Route>();
            RouteList = new ObservableRangeCollection<Route>();
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
                await GetRouteListAsync();
            }
        }

        private async Task GetRouteListAsync()
        {
            try
            {
                string url = "routes";
                var routlist = await _genericService.GetPagedListAsync(url);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    RouteList.Clear();
                    RouteList.AddRange(routlist.Data.Data);
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
