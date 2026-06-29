using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Views;

namespace UTeM_Mobile.Services
{
    public class AppNavigationService : IAppNavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task GoToLoginAsync()
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            Application.Current!.MainPage = new NavigationPage(loginPage);
            return Task.CompletedTask;
        }

        public Task GoToGuardShellAsync()
        {
            var shell = _serviceProvider.GetRequiredService<GuardShell>();

            Application.Current!.MainPage = shell;

            return Task.CompletedTask;
        }

        public Task GoToSupervisorShellAsync()
        {
            var shell = _serviceProvider.GetRequiredService<SupervisorShell>();

            Application.Current!.MainPage = shell;

            return Task.CompletedTask;
        }
    }
}
