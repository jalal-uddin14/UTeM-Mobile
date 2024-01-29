using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class LogoutService
    {
        public static async Task LogoutAsync()
        {
            await TimerDBService.Delete();
            await LocalDBService.RemoveToken();
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
