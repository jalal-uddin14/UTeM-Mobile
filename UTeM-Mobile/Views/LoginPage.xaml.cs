using UTeM_Mobile.Interfaces;
using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}
    private void Entry_Completed(object sender, EventArgs e)
    {
        try
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }
        catch (Exception)
        {

        }
    }
}