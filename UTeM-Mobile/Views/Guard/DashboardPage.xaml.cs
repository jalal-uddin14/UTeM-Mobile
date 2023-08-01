using UTeM_Mobile.StaticProperties;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel viewModel;
    private bool isFlashOn;
	public DashboardPage()
	{
		InitializeComponent();
        isFlashOn = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as DashboardViewModel;
        viewModel.OnAppearing();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!isFlashOn)
            {
                await Flashlight.Default.TurnOnAsync();
                isFlashOn = true;
            }
            else
            {
                await Flashlight.Default.TurnOffAsync();
                isFlashOn = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }
}