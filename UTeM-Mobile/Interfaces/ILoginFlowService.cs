using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface ILoginFlowService
    {
        Task LoginAsync(AuthToken request);
        Task RestoreSessionAsync();
    }
}
