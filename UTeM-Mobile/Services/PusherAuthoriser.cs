using System.Net.Http.Headers;
using PusherClient;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Core.Models;

namespace healholmes_xamarin.Services
{
    public class PusherAuthoriser : IAuthorizer
    {
        private AuthToken token;
        public PusherAuthoriser(AuthToken token)
        {
            this.token = token;
        }
        public string Authorize(string channelName, string socketId)
        {
            var authorizer = new HttpAuthorizer(ServerCredential.BaseUrl + "broadcasting/users/auth")
            {
                AuthenticationHeader = new AuthenticationHeaderValue("Authorization", "Bearer " + this.token.Token),
            };
            return authorizer.Authorize(channelName, socketId);
        }
    }
}
