using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolDetailListPage : ContentPage
{
	private PatrolDetailListViewModel viewModel;
	public PatrolDetailListPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as PatrolDetailListViewModel;
        viewModel.OnAppearing();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as PatrolDetail;
        await Shell.Current.GoToAsync($"{nameof(PatrolDetailPage)}?Id={Item.Id}");
    }
}