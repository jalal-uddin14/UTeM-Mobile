using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;

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
			});
        builder.Services.AddTransient<IGenericService<ApplicationUser>, GenericService<ApplicationUser>>();
        builder.Services.AddTransient<IGenericService<Route>, GenericService<Route>>();
        builder.Services.AddTransient<IGenericService<Patrol>, GenericService<Patrol>>();
        builder.Services.AddTransient<IGenericService<AuthToken>, GenericService<AuthToken>>();
        return builder.Build();
	}
}
