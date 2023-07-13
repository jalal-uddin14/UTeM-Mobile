using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Data.StaticCredentials;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class RouteListViewModel : BaseViewModel
    {
        private string _baseUrl;
        private IGenericService<Route> _genericService;
        private ApplicationUser LoggedinUser;
        public ObservableRangeCollection<Route> RouteList { get; }

        public RouteListViewModel()
        {
            _baseUrl = ServerCredential.BaseUrl;
            RouteList = new ObservableRangeCollection<Route>();
        }

        public void OnAppearing()
        {
            _genericService = new GenericService<Route>();
            Task.Run(async () => await GetRouteListAsync());
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
