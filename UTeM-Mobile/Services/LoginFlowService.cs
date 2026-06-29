using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class LoginFlowService : ILoginFlowService
    {
        private readonly IGenericService<AuthToken> _authApi;
        private readonly IAuthenticationService _authService;
        private readonly ITokenStorageService _tokenStorage;
        private readonly INFCService _nfcService;
        private readonly IPusherService _pusherService;
        private readonly ITimeOutService _timeOutService;
        private readonly IAppNavigationService _navigationService;

        public LoginFlowService(
            IGenericService<AuthToken> authApi,
            IAuthenticationService authService,
            ITokenStorageService tokenStorage,
            INFCService nfcService,
            IPusherService pusherService,
            ITimeOutService timeOutService,
            IAppNavigationService navigationService
        ) {
            _authApi = authApi;
            _authService = authService;
            _tokenStorage = tokenStorage;
            _nfcService = nfcService;
            _pusherService = pusherService;
            _timeOutService = timeOutService;
            _navigationService = navigationService;
        }

        public async Task LoginAsync(ApplicationUser user, bool isRemember)
        {
            ObjectResponse<AuthToken> response = await _authApi.PostAsync("accounts/login", user);

            if (response is null || response.Data is null)
            {
                throw new Exception("Invalid login response.");
            }
            if (!response.IsSuccess)
            {
                throw new Exception(response.Message);
            }
            AuthToken token = response.Data;
            token.IsRemember = isRemember;
            await _tokenStorage.SaveAccessTokenAsync(JsonSerializer.Serialize(response.Data));
            _authService.SetSession(response.Data);

            await NavigateByRoleAsync(response.Data);
        }

        public async Task RestoreSessionAsync()
        {
            string tokenJson = await _tokenStorage.GetAccessTokenAsync();
            AuthToken? token = JsonSerializer.Deserialize<AuthToken>(tokenJson);

            if (token is null || !token.IsRemember || token.ValidTo <= DateTime.Now)
            {
                await _navigationService.GoToLoginAsync();
                return;
            }

            _authService.SetSession(token);

            await NavigateByRoleAsync(token);
        }

        private async Task NavigateByRoleAsync(AuthToken token)
        {
            if (token.UserRole == "Supervisor")
            {
                await _pusherService.SubscribeGuardChannelAsync();
                await _navigationService.GoToSupervisorShellAsync();
                return;
            }

            if (token.UserRole == "Guard")
            {
                await _nfcService.InitializeAsync();
                await _timeOutService.CheckTimerTokenAsync();
                await _navigationService.GoToGuardShellAsync();
                return;
            }

            await _navigationService.GoToLoginAsync();
        }
    }
}
