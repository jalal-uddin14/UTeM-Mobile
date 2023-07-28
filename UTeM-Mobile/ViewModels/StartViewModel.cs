using MvvmHelpers;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;

namespace UTeM_Mobile.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private ApplicationUser user;

        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public StartViewModel()
        {
            User = new ApplicationUser();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        private async Task GetTokenAsync()
        {
            //await LocalDBService.RemoveToken();
            await LocalDBService.InitDB();
            AuthToken token = await LocalDBService.GetToken();
            if (token != null && token.IsRemember && token.ValidTo > DateTime.Now)
            {
                if (token.UserRole == "Supervisor")
                {
                    await PusherService.SubscribeGuardChannel();
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new SupervisorShell();
                    });
                }
                else if (token.UserRole == "Guard")
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new GuardShell();
                    });
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("LoginPage"));
            }
        }
    }
}
