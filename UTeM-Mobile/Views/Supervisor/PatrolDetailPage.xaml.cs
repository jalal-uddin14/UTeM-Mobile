using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolDetailPage : ContentPage
{
	private PatrolDetailViewModel viewModel;
	public PatrolDetailPage()
	{
		InitializeComponent();
        viewModel = BindingContext as PatrolDetailViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
    }
}