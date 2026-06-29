using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
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

        public async Task LoginAsync(AuthToken request)
        {
            ObjectResponse<AuthToken> token = await _authApi.PostAsync("Auth/Login", request);

            if (token is null)
                throw new Exception("Invalid login response.");

            await _tokenStorage.SaveAccessTokenAsync(JsonSerializer.Serialize(token.Data));

            _authService.SetSession(token.Data);

            await NavigateByRoleAsync(token.Data);
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
