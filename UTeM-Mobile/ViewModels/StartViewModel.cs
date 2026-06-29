using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.NFC;
using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;
using UTeM_Mobile.StaticProperties;
using UTeM_Mobile.Views;

namespace UTeM_Mobile.ViewModels
{
    public partial class StartViewModel : ObservableObject, IOnAppearing
    {
        private readonly ITokenStorageService _tokenService;
        private readonly INFCService _nfcService;
        private readonly ITimeOutService _timeOutService;

        public StartViewModel(ITokenStorageService tokenService, INFCService nFCService, ITimeOutService timeOutService)
        {
            _tokenService = tokenService;
            _nfcService = nFCService;
            _timeOutService = timeOutService;
        }

        public async Task OnAppearing()
        {
            await GetTokenAsync();
        }

        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(tokenJson))
                {
                    await NavigateToLoginAsync();
                    return;
                }

                AuthToken? token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
                if (token == null || !token.IsRemember || token.ValidTo <= DateTime.Now)
                {
                    _tokenService.RemoveAccessToken();
                    await NavigateToLoginAsync();
                    return;
                }

                if (token.UserRole == "Supervisor")
                {
                    await PusherService.SubscribeGuardChannel();
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current!.MainPage = new SupervisorShell();
                    });
                    return;
                }

                if (token.UserRole == "Guard")
                {
                    if (!CrossNFC.Current.IsAvailable)
                    {
                        StaticMessage.HasNFCMessage = true;
                        StaticMessage.NFCMessage = "NFC is not available in your phone.";
                    }
                    else if (!CrossNFC.Current.IsEnabled)
                    {
                        StaticMessage.HasNFCMessage = true;
                        StaticMessage.NFCMessage = "Please turn on NFC.";
                    }
                    else
                    {
                        _nfcService.SubscribeNFC();
                    }

                    await _timeOutService.CheckTimerToken();
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current!.MainPage = new GuardShell();
                    });
                    return;
                }

                _tokenService.RemoveAccessToken();
                await NavigateToLoginAsync();
            }
            catch (Exception ex)
            {
                StaticMessage.HasNFCMessage = true;
                StaticMessage.NFCMessage = ex.ToString();
                await NavigateToLoginAsync();
            }
        }

        private async Task NavigateToLoginAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.GoToAsync(nameof(LoginPage)));
        }
    }
}
