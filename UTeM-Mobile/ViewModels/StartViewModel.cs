using MvvmHelpers;
using Plugin.NFC;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;

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
            try
            {
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
                        if (!CrossNFC.Current.IsAvailable)
                        {
                            StaticMessage.HasNFCMessage = true;
                            StaticMessage.NFCMessage = "NFC is not available in your phone.";
                        }
                        else if (!CrossNFC.Current.IsEnabled)
                        {
                            StaticMessage.HasNFCMessage = true;
                            StaticMessage.NFCMessage = "Please turn on NFC.";
                        }
                        else
                        {
                            NFCService.SubscribeNFC();
                        }
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
            catch(Exception ex)
            {

            }
        }
    }
}
