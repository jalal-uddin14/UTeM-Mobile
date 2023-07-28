using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class WhatsAppPage : ContentPage
{
    private WhatsAppViewModel viewModel;
	public WhatsAppPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as WhatsAppViewModel;
        viewModel.OnAppearing();
    }
}