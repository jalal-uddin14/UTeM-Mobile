using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class GuardDetailPage : ContentPage
{
	private GuardDetailViewModel viewModel;
	public GuardDetailPage()
	{
		InitializeComponent();
		viewModel = BindingContext as GuardDetailViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.OnAppearing();
    }
}