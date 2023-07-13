using UTeM_Mobile.ViewModels;

namespace UTeM_Mobile.Views;

public partial class StartPage : ContentPage
{
	private StartViewModel viewModel;
	public StartPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel = BindingContext as StartViewModel;
		viewModel.OnAppearing();
    }
}