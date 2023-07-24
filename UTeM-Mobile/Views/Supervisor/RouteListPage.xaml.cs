 using UTeM_Mobile.Data.Models;
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

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as Route;
        await Shell.Current.GoToAsync($"{nameof(RouteDetailPage)}?Id={Item.Id}");
    }
}