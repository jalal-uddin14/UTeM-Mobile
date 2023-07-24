using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile;

public partial class GuardShell : Shell
{
	public GuardShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(PatrolListPage), typeof(PatrolListPage));
		Routing.RegisterRoute(nameof(PatrolDetailPage), typeof(PatrolDetailPage));
		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
	}

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}