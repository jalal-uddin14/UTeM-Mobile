using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolAddPage : ContentPage
{
	private PatrolAddViewModel viewModel;
	public PatrolAddPage()
	{
		InitializeComponent();
		viewModel = BindingContext as PatrolAddViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.OnAppearing();
    }
}