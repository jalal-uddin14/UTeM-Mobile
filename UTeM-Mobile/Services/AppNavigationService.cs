using UTeM_Mobile.Interfaces;

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
            return Shell.Current.GoToAsync("//LoginPage");
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
