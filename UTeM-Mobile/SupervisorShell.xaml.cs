using UTeM_Mobile.Views.Supervisor;

namespace UTeM_Mobile;

public partial class SupervisorShell : Shell
{
	public SupervisorShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(GuardDetailPage), typeof(GuardDetailPage));
        Routing.RegisterRoute(nameof(RouteDetailPage), typeof(RouteDetailPage));
        Routing.RegisterRoute(nameof(PatrolAddPage), typeof(PatrolAddPage));
        Routing.RegisterRoute(nameof(PatrolDetailPage), typeof(PatrolDetailPage));
    }
}