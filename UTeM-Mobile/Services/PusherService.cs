using PusherClient;
using UTeM_Mobile.Data.StaticCredentials;
using Newtonsoft.Json;
using healholmes_xamarin.Services;
using UTeM_Mobile.Core.Models;
using Nito.AsyncEx;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public static class PusherService
    {
        public static AuthToken token = AsyncContext.Run(LocalDBService.GetToken);
        public static Pusher pusher = new Pusher(PusherCredential.key, new PusherOptions
        {
            Cluster = PusherCredential.cluster,
            Encrypted = true
        });
        public static Channel PrivateChannel;

        private static void GetPusher()
        {
            if (pusher == null)
            {
                pusher = new Pusher(PusherCredential.key, new PusherOptions
                {
                    Cluster = PusherCredential.cluster,
                    Encrypted = true
                });
            }
        }

        public static async Task SubscribeGuardChannel()
        {
            try
            {
                if (!await StaticMessage.ShowInternetMessage())
                {
                    return;
                }
                GetPusher();
                await pusher.ConnectAsync().ConfigureAwait(false);
                PrivateChannel = await pusher.SubscribeAsync("UTeM-Guard").ConfigureAwait(false);
                PrivateChannel.Bind("guard.activities." + token.UserId, GuardActivityListener);
            }
            catch (Exception ex)
            {
                if (!await StaticMessage.ShowInternetMessage())
                {
                    return;
                }
            }

        }

        public static async void GuardActivityListener(object sender)
        {
            try
            {
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(sender));
                Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(dictionary["data"]);
                var request = new NotificationRequest
                {
                    Title = response["title"],
                    Subtitle = response["message"],
                    Description = response["description"],
                    Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                    {
                        VisibilityType = Plugin.LocalNotification.AndroidOption.AndroidVisibilityType.Public,
                    }
                };
                PopMessage popMessage = new PopMessage
                {
                    Type = response["type"],
                    Heading = response["title"],
                    Title = response["message"],
                    Message = response["description"]
                };
                if (response["type"] == "SoS")
                {
                    popMessage.NavigateTo = "ReportListPage";
                    popMessage.HasNavigate = true;
                }
                else if (response["type"] == "Patrol")
                {
                    popMessage.NavigateTo = "PatrolListPage";
                    popMessage.HasNavigate = true;
                }
                
                await LocalNotificationCenter.Current.Show(request);
                LocalNotificationCenter.Current.NotificationActionTapped += NoficationAction_Tapped;
                await Application.Current.MainPage.Navigation.PopToRootAsync(true);
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popMessage))
                );
            }
            catch(Exception ex)
            {

            }
        }

        private static async void NoficationAction_Tapped(NotificationActionEventArgs e)
        {
            if (e.IsTapped)
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//ReportListPage"));
            }
        }
    }
}
