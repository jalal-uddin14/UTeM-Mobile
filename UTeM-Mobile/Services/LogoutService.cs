using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.Services
{
    public class LogoutService
    {
        public static async Task LogoutAsync()
        {
            await LocalDBService.RemoveToken();
            await PatrolDBService.Delete();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
    }
}
