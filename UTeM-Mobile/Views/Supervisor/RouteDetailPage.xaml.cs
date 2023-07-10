using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class RouteDetailPage : ContentPage
{
	private RouteDetailViewModel viewModel;
	public RouteDetailPage()
	{
		InitializeComponent();
        viewModel = BindingContext as RouteDetailViewModel;
    }

	public RouteDetailPage(int id)
	{
        InitializeComponent();
        viewModel = BindingContext as RouteDetailViewModel;
        viewModel.Id = id;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
    }
}