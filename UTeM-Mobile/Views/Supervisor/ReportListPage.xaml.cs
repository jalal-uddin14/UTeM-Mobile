using UTeM_Mobile.Data.Models;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class ReportListPage : ContentPage
{
	private ReportListViewModel viewModel;
	public ReportListPage()
	{
		InitializeComponent();
		viewModel = BindingContext as ReportListViewModel;
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
        var Item = e.Item as Report;
        await Shell.Current.GoToAsync($"{nameof(ReportDetailPage)}?Id={Item.Id}");
    }
}