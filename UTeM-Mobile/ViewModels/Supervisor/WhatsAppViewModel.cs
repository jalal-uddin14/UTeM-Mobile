using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class WhatsAppViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<ApplicationUser> _genericUserService;
        private readonly ITokenStorageService _tokenService;
        private ApplicationUser user;
        private Patrol patrol;
        private string phoneNumber;

        public WhatsAppViewModel(ITokenStorageService tokenService)
        {
            _genericPatrolService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
            _tokenService = tokenService;
        }
        public async Task GetTokenAsync()
        {
            var tokenJson = await _tokenService.GetAccessTokenAsync();
            //if (Token != null)
            //{
            //    await GetProfileAsync();
            //}
        }

        public async Task OnAppearing()
        {
            await GetTokenAsync();
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    user = response.Data;
                    phoneNumber = user.PhoneNumber;
                }
                else
                {
                    await GetPatrolList();
                }
                
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
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
                string url = "patrols?date=" + DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");
                PaginatedResponse<Patrol> response = await _genericPatrolService.GetPagedListAsync(url, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Count > 0)
                {
                    patrol = response.Data.Data.FirstOrDefault();
                    phoneNumber = patrol.Guard.PhoneNumber;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetNavigationMessage("Patrol", "Internal error occured", "PatrolListPage")))
                    );
                }
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
            }
            finally
            {
                //await OpenWhatsApp();
            }
        }
        private async Task OpenWhatsApp()
        {
            try
            {
                await Launcher.Default.OpenAsync($"whatsapp://send?phone=+60{phoneNumber}");
            }
            catch (Exception)
            {

            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//PatrolListPage"));
            }
        }
    }
}
