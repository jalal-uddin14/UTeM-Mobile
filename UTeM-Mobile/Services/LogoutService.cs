using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class LogoutService : ILogoutService
    {
        private readonly ITokenStorageService _storageService;
        public LogoutService(ITokenStorageService storageService)
        {
            _storageService = storageService;
        }
        public async Task LogoutAsync()
        {
            await TimerDBService.Delete();
            _storageService.RemoveAccessToken();
            await PatrolDBService.Delete();
            StaticCredentials.PatrolDetail = null;
            StaticCredentials.CheckpointTimer = null;
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
    }
}
