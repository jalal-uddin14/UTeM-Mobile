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

        builder.Services.AddSingleton<IGenericService<ApplicationUser>, GenericService<ApplicationUser>>();
        builder.Services.AddSingleton<IGenericService<Route>, GenericService<Route>>();
        builder.Services.AddSingleton<IGenericService<Patrol>, GenericService<Patrol>>();
        builder.Services.AddSingleton<IGenericService<PatrolDetail>, GenericService<PatrolDetail>>();
        builder.Services.AddSingleton<IGenericService<PatrolCheckpoint>, GenericService<PatrolCheckpoint>>();
        builder.Services.AddSingleton<IGenericService<Report>, GenericService<Report>>();
        builder.Services.AddSingleton<IGenericService<AuthToken>, GenericService<AuthToken>>();

		builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
		builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
		builder.Services.AddSingleton<ILogoutService, LogoutService>();
		builder.Services.AddSingleton<IPatrolService, PatrolService>();
		builder.Services.AddSingleton<INFCService, NFCService>();
		builder.Services.AddSingleton<ITimeOutService, TimeOutService>();
        builder.Services.AddSingleton<LocalDBService>();

        builder.Services.AddTransient<AppShell>();

        builder.Services.AddTransient<StartPage>();
        builder.Services.AddTransient<StartViewModel>();
        builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PatrolDetailListPage>();
        builder.Services.AddTransient<PatrolDetailListViewModel>();
        builder.Services.AddTransient<PatrolDetailPage>();
        builder.Services.AddTransient<PatrolDetailViewModel>();
        builder.Services.AddTransient<PatrolListPage>();
        builder.Services.AddTransient<PatrolListViewModel>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ReportSendPage>();
        builder.Services.AddTransient<ReportSendViewModel>();


        return builder.Build();
	}
}
