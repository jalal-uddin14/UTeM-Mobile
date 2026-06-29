using Plugin.LocalNotification;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Services;
using UTeM_Mobile.ViewModels;
using UTeM_Mobile.ViewModels.Guard;
using UTeM_Mobile.Views;
using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiMaps()
            .UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa-brands-400.ttf", "FAbrands");
				fonts.AddFont("fa-regular-400.ttf", "FAregular");
				fonts.AddFont("fa-solid-900.ttf", "FAsolid");
				fonts.AddFont("fa-v4compatibility.ttf", "FAv4");
			})
			.UseLocalNotification();

        builder.Services
            .AddSingleton<IGenericService<ApplicationUser>, GenericService<ApplicationUser>>()
            .AddSingleton<IGenericService<Route>, GenericService<Route>>()
            .AddSingleton<IGenericService<Patrol>, GenericService<Patrol>>()
            .AddSingleton<IGenericService<PatrolDetail>, GenericService<PatrolDetail>>()
            .AddSingleton<IGenericService<PatrolCheckpoint>, GenericService<PatrolCheckpoint>>()
            .AddSingleton<IGenericService<Report>, GenericService<Report>>()
            .AddSingleton<IGenericService<AuthToken>, GenericService<AuthToken>>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<ITokenStorageService, TokenStorageService>()
            .AddSingleton<ILogoutService, LogoutService>()
            .AddSingleton<IPatrolService, PatrolService>()
            .AddSingleton<INFCService, NFCService>()
            .AddSingleton<ITimeOutService, TimeOutService>()
            .AddSingleton<IPusherService, PusherService>()
            .AddSingleton<ILoginFlowService, LoginFlowService>()
            .AddSingleton<IAppNavigationService, AppNavigationService>()
            .AddSingleton<IAppStartupService, AppStartupService>()
            .AddSingleton<LocalDBService>()
            .AddSingleton<IDialogService, DialogService>();

        builder.Services.AddTransient<AppShell>()
            .AddTransient<GuardShell>()
            .AddTransient<SupervisorShell>()
            .AddTransient<StartPage>()
            .AddTransient<StartViewModel>()
            .AddTransient<LoginPage>()
            .AddTransient<LoginViewModel>()
            .AddTransient<DashboardPage>()
            .AddTransient<DashboardViewModel>()
            .AddTransient<PatrolDetailListPage>()
            .AddTransient<PatrolDetailListViewModel>()
            .AddTransient<PatrolDetailPage>()
            .AddTransient<PatrolDetailViewModel>()
            .AddTransient<PatrolListPage>()
            .AddTransient<PatrolListViewModel>()
            .AddTransient<ProfilePage>()
            .AddTransient<ProfileViewModel>()
            .AddTransient<ReportSendPage>()
            .AddTransient<ReportSendViewModel>();


        return builder.Build();
	}
}
