using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class AppStartupService : IAppStartupService
    {
        private readonly ITokenStorageService _tokenStorageService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INFCService _nfcService;
        private readonly ITimeOutService _timeOutService;
        private readonly IPusherService _pusherService;
        private readonly IAppNavigationService _navigationService;

        public AppStartupService(
            ITokenStorageService tokenStorageService,
            IAuthenticationService authenticationService,
            INFCService nfcService,
            ITimeOutService timeOutService,
            IPusherService pusherService,
            IAppNavigationService navigationService)
        {
            _tokenStorageService = tokenStorageService;
            _authenticationService = authenticationService;
            _nfcService = nfcService;
            _timeOutService = timeOutService;
            _pusherService = pusherService;
            _navigationService = navigationService;
        }

        public async Task RestoreSessionAsync()
        {
            try
            {
                var tokenJson = await _tokenStorageService.GetAccessTokenAsync();

                if (string.IsNullOrWhiteSpace(tokenJson))
                {
                    await _navigationService.GoToLoginAsync();
                    return;
                }

                var token = JsonSerializer.Deserialize<AuthToken>(tokenJson);

                if (token is null || !token.IsRemember || token.ValidTo <= DateTime.Now)
                {
                    _tokenStorageService.RemoveAccessToken();
                    await _navigationService.GoToLoginAsync();
                    return;
                }

                _authenticationService.SetSession(token);

                await ContinueByRoleAsync(token);
            }
            catch(Exception ex)
            {
                _tokenStorageService.RemoveAccessToken();
                await _navigationService.GoToLoginAsync();
            }
        }

        private async Task ContinueByRoleAsync(AuthToken token)
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

            _tokenStorageService.RemoveAccessToken();
            await _navigationService.GoToLoginAsync();
        }
    }
}
