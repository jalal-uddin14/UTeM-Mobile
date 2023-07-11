using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

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
		Console.WriteLine("Reached here");
		viewModel.OnAppearing();
    }
}