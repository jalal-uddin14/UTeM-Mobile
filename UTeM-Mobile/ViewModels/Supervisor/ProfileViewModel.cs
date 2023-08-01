using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class ProfileViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericService;
        private ApplicationUser user;
        private AuthToken token;

        public ICommand UpdateProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public AuthToken Token { get => token; set => SetProperty(ref token, value); }

        public ProfileViewModel()
        {
            User = new ApplicationUser();
            _genericService = new GenericService<ApplicationUser>();
            UpdateProfileCommand = new AsyncCommand(ExecuteUpdateProfile);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }
        private async Task ExecuteLogout()
        {
            try
            {
                await LocalDBService.RemoveToken();
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ExecuteUpdateProfile()
        {
            try
            {
                string url = "accounts/update";
                ObjectResponse<ApplicationUser> response = await _genericService.PutAsync(url, User, token);
                IsSuccessMessage = response.IsSuccess;
                if (IsSuccessMessage)
                {
                    Message = response.Message;
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
                IsSuccessMessage = false;
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                    {
                        { "Heading", "Error" },
                        { "Title", "Internal error occured" },
                        { "Message", "" },
                        { "NavigateTo", "" },
                        { "HasNavigate", "" },
                    };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
            }
            finally
            {
                await GetProfileAsync();
                DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            Token = await LocalDBService.GetToken();
            if (Token != null)
            {
                await GetProfileAsync();
            }
        }

        private async Task GetProfileAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
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
    }
}
