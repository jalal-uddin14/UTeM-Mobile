using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile;

public partial class GuardShell : Shell
{
	public GuardShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(PatrolDetailPage), typeof(PatrolDetailPage));
	}
}