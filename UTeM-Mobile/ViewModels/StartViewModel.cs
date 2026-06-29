using CommunityToolkit.Mvvm.ComponentModel;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels
{
    public partial class StartViewModel : ObservableObject, IOnAppearing
    {
        private readonly IAppStartupService _appStartupService;

        public StartViewModel(IAppStartupService appStartupService)
        {
            _appStartupService = appStartupService;
        }

        public async Task OnAppearing()
        {
            await _appStartupService.RestoreSessionAsync();
        }
    }
}
