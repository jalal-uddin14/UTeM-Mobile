using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class WhatsAppViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;
        private Patrol patrol;
        private string phoneNumber;

        public WhatsAppViewModel()
        {
            _genericPatrolService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
        }
        public async Task GetTokenAsync()
        {
            token = await LocalDBService.GetToken();
            if (token != null)
            {
                await GetProfileAsync();
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
                user = response.Data;
                phoneNumber = user.PhoneNumber;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await GetPatrolList();
            }
        }

        private async Task GetPatrolList()
        {
            try
            {
                string url = "patrols?date=" + DateTime.Now.Date;
                PaginatedResponse<Patrol> response = await _genericPatrolService.GetPagedListAsync(url, token);
                if (response.IsSuccess && response.Data != null && response.Data.Count > 0)
                {
                    patrol = response.Data.Data.FirstOrDefault();
                    phoneNumber = patrol.Guard.PhoneNumber;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await OpenWhatsApp();
            }
        }
        private async Task OpenWhatsApp()
        {
            try
            {
                await Launcher.Default.OpenAsync($"whatsapp://send?phone=+60{phoneNumber}");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//PatrolListPage"));
            }
        }
    }
}
