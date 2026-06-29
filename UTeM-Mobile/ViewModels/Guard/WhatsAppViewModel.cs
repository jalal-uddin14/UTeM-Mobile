using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;
using System.Text.Json;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class WhatsAppViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericUserService;
        private readonly ITokenStorageService _tokenService;
        private string phoneNumber;

        public WhatsAppViewModel(ITokenStorageService tokenService)
        {
            _genericUserService = new GenericService<ApplicationUser>();
            _tokenService = tokenService;
        }

        public async Task OnAppearing()
        {
            IsErrorMessage = false;
            await GetTokenAsync();
        }
        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                Token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
                if (Token != null)
                {
                    await GetProfileAsync();
                }
            }
            catch(Exception)
            {

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
                    url = "supervisors/" + response.Data.SupervisorId;
                    ObjectResponse<ApplicationUser> response1 = await _genericUserService.GetDetailsAsync(url, Token);
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
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
            //finally
            //{
            //    await OpenWhatsApp();
            //}
        }
        private async Task OpenWhatsApp()
        {
            try
            {
                await Launcher.Default.OpenAsync($"whatsapp://send?phone=+60{phoneNumber}");
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage("Error openning whatsapp")))
                );
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//DashboardPage"));
            }
        }
    }
}
