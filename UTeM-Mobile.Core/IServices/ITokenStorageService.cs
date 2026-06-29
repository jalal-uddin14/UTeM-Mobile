namespace UTeM_Mobile.Core.IServices
{
    public interface ITokenStorageService
    {
        Task SaveAccessTokenAsync(string token);

        Task<string?> GetAccessTokenAsync();

        void RemoveAccessToken();

        void Clear();
    }
}
