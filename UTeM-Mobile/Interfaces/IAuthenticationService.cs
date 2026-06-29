using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface IAuthenticationService
    {
        ApplicationUser? CurrentUser { get; }
        AuthToken? CurrentToken { get; }
        bool IsLoggedIn { get; }
        public void SetSession(ApplicationUser user, AuthToken token);
        public void Logout();
    }
}
