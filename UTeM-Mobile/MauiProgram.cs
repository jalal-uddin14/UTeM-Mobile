using Plugin.LocalNotification;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;

namespace UTeM_Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
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
			.UseLocalNotification()
			.UseMauiMaps();
        builder.Services.AddTransient<IGenericService<ApplicationUser>, GenericService<ApplicationUser>>();
        builder.Services.AddTransient<IGenericService<Route>, GenericService<Route>>();
        builder.Services.AddTransient<IGenericService<Patrol>, GenericService<Patrol>>();
        builder.Services.AddTransient<IGenericService<PatrolCheckpoint>, GenericService<PatrolCheckpoint>>();
        builder.Services.AddTransient<IGenericService<Report>, GenericService<Report>>();
        builder.Services.AddTransient<IGenericService<AuthToken>, GenericService<AuthToken>>();
        return builder.Build();
	}
}
