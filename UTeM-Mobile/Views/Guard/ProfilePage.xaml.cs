using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class ProfilePage : ContentPage
{
	private ProfileViewModel viewModel;
	public ProfilePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel = BindingContext as ProfileViewModel;
		viewModel.OnAppearing();
    }
}