using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface ILoginFlowService
    {
        Task LoginAsync(ApplicationUser user, bool isRemember);
        Task RestoreSessionAsync();
    }
}
