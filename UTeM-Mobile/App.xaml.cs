namespace UTeM_Mobile;

public partial class App : Application
{
	public App(AppShell appShell)
	{
        InitializeComponent();
        Application.Current.UserAppTheme = AppTheme.Light;
        MainPage = appShell;
	}
}
