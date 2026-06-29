using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.Core.IServices
{
    public interface ILocalDBService
    {
        Task InsertTokenAsync(AuthToken token);
        Task<AuthToken?> GetTokenAsync();
        Task RemoveTokenAsync();
    }
}
