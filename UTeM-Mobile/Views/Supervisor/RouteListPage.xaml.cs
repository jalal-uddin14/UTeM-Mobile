using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class RouteListPage : ContentPage
{
	private RouteListViewModel viewModel;
	public RouteListPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as RouteListViewModel;
        viewModel.OnAppearing();
    }
}