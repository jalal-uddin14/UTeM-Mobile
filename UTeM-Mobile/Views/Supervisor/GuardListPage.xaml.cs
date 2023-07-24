using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class GuardListPage : ContentPage
{
	private GuardListViewModel viewModel;
	public GuardListPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as GuardListViewModel;
        viewModel.OnAppearing();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
        var Item = e.Item as ApplicationUser;
        await Shell.Current.GoToAsync($"{nameof(GuardDetailPage)}?Id={Item.Id}");
    }
}