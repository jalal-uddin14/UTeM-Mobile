using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

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