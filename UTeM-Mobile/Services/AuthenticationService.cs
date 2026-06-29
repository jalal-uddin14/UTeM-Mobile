using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public ApplicationUser? CurrentUser { get; private set; }

        public AuthToken? CurrentToken { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public void SetSession(ApplicationUser user, AuthToken token)
        {
            CurrentUser = user;
            CurrentToken = token;
        }

        public void Logout()
        {
            CurrentUser = null;
            CurrentToken = null;
        }
    }
}
