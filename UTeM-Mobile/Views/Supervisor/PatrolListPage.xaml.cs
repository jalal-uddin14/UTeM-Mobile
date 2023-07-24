using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Supervisor;
using DeviceDisplay = Microsoft.Maui.Devices.DeviceDisplay;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolListPage : ContentPage
{
	private PatrolListViewModel viewModel;
	public PatrolListPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as PatrolListViewModel;
        viewModel.OnAppearing();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as Patrol;
        await Shell.Current.GoToAsync($"{nameof(PatrolDetailPage)}?Id={Item.Id}");
    }
}