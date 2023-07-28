using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class WhatsAppViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;
        private string phoneNumber;

        public WhatsAppViewModel()
        {
            _genericUserService = new GenericService<ApplicationUser>();
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
                await GetProfileAsync();
            }
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, token);
                if (response.IsSuccess && response.Data != null)
                {
                    url = "supervisors/" + response.Data.SupervisorId;
                    ObjectResponse<ApplicationUser> response1 = await _genericUserService.GetDetailsAsync(url, token);
                    if (response.IsSuccess && response1.Data != null)
                    {
                        phoneNumber = response1.Data.PhoneNumber;
                    }
                    else
                    {
                        phoneNumber = response.Data.PhoneNumber;
                    }
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
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//DashboardPage"));
            }
        }
    }
}
