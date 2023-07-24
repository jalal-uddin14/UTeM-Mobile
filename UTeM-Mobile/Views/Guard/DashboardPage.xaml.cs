using UTeM_Mobile.ViewModels.Guard;
using Flashlight = Xamarin.Essentials.Flashlight;

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
                await Flashlight.TurnOnAsync();
                isFlashOn = true;
            }
            else
            {
                await Flashlight.TurnOffAsync();
                isFlashOn = false;
            }
        }
        catch (Exception ex)
        {
            // Unable to turn on/off flashlight
        }
    }
}