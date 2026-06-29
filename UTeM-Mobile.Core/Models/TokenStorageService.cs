using Microsoft.Maui.Storage;
using UTeM_Mobile.Core.IServices;

namespace UTeM_Mobile.Core.Models
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string AccessTokenKey = "access_token";

        public async Task SaveAccessTokenAsync(string token)
        {
            await SecureStorage.Default.SetAsync(AccessTokenKey, token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(AccessTokenKey);
        }

        public void RemoveAccessToken()
        {
            SecureStorage.Default.Remove(AccessTokenKey);
        }

        public void Clear()
        {
            SecureStorage.Default.RemoveAll();
        }
    }
}
