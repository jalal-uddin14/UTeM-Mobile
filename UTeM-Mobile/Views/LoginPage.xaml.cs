using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.Views;

public partial class LoginPage : ContentPage
{
	private LoginViewModel viewModel;
	public LoginPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel = BindingContext as LoginViewModel;
		viewModel.OnAppearing();
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        viewModel.IsRemember = !viewModel.IsRemember;
    }
}