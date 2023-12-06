using UTeM_Mobile.Views.Supervisor;

namespace UTeM_Mobile;

public partial class SupervisorShell : Shell
{
	public SupervisorShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RouteListPage), typeof(RouteListPage));
        Routing.RegisterRoute(nameof(GuardListPage), typeof(GuardListPage));

        Routing.RegisterRoute(nameof(GuardDetailPage), typeof(GuardDetailPage));
        Routing.RegisterRoute(nameof(RouteDetailPage), typeof(RouteDetailPage));
        Routing.RegisterRoute(nameof(PatrolAddPage), typeof(PatrolAddPage));
        Routing.RegisterRoute(nameof(PatrolDetailListPage), typeof(PatrolDetailListPage));
        Routing.RegisterRoute(nameof(PatrolDetailPage), typeof(PatrolDetailPage));
        Routing.RegisterRoute(nameof(ReportDetailPage), typeof(ReportDetailPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var phoneNumber = "01924241969";
        try
        {
            await Launcher.Default.OpenAsync($"whatsapp://send?phone=+88{phoneNumber}");
        }
        catch (Exception)
        {

        }
    }
}