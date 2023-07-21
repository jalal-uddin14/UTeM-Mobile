using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel viewModel;
	public DashboardPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as DashboardViewModel;
        viewModel.OnAppearing();
    }
}