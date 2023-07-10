using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolListPage : ContentPage
{
	private PatrolListViewModel viewModel;
	public PatrolListPage()
	{
		InitializeComponent();
		viewModel = BindingContext as PatrolListViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var Item = e.Item as Patrol;
        await Shell.Current.GoToAsync($"{nameof(PatrolDetailPage)}?Id={Item.Id}");
    }
}