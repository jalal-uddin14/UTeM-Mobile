using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile;

public partial class GuardShell : Shell
{
	public GuardShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("GuardPatrolListPage", typeof(PatrolListPage));
		Routing.RegisterRoute("GuardPatrolDetailPage", typeof(PatrolDetailPage));
		Routing.RegisterRoute("GuardProfilePage", typeof(ProfilePage));
	}

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}