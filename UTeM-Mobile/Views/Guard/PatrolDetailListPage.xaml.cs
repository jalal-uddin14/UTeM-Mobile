using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class PatrolDetailListPage : ContentPage
{
	private PatrolDetailListViewModel viewModel;
	public PatrolDetailListPage()
	{
		InitializeComponent();
        viewModel = BindingContext as PatrolDetailListViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as PatrolDetail;
        await Shell.Current.GoToAsync($"GuardPatrolDetailPage?Id={Item.Id}");
    }
}