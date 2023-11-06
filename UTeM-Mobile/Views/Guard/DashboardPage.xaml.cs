using Microsoft.Maui.Maps;
using UTeM_Mobile.Services;
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

    public async Task GetCurrentLocation()
    {
        try
        {
            var location = await LocationService.GetCurrentLocationAsync();
            if (location != null)
            {
                map.MoveToRegion(mapSpan: MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMeters(500)));
            }
        }
        catch (Exception)
        {
            // Unable to get location
        }
        finally
        {
            
        }
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

    private async void map_Loaded(object sender, EventArgs e)
    {
        await GetCurrentLocation();
    }
}