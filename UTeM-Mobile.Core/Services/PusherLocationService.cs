using PusherServer;
using UTeM_Mobile.Data.StaticCredentials;

namespace UTeM_Mobile.Core.Services
{
    public class PusherLocationService
    {
        private static IPusher _pusher;
        public static async Task<bool> SendNotificationAsync(string channelName, string eventName, object data)
        {
            try
            {
                var options = new PusherOptions
                {
                    Cluster = PusherCredential.cluster,
                    Encrypted = true
                };

                _pusher = new Pusher(PusherCredential.app_id, PusherCredential.key, PusherCredential.secret, options);

                var result = await _pusher.TriggerAsync(channelName, eventName, data);
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
