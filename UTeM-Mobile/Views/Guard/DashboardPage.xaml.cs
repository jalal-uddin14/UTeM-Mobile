using Microsoft.Maui.Maps;
using UTeM_Mobile.StaticProperties;
using UTeM_Mobile.ViewModels.Guard;

namespace UTeM_Mobile.Views.Guard;

public partial class DashboardPage : ContentPage
{
    private CancellationTokenSource _cancelTokenSource;

    private DashboardViewModel viewModel;
    private bool isFlashOn;
	public DashboardPage()
	{
		InitializeComponent();
        isFlashOn = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as DashboardViewModel;
        viewModel.OnAppearing();
        await GetCurrentLocation();
    }

    public async Task GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
            _cancelTokenSource = new CancellationTokenSource();
            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                map.MoveToRegion(mapSpan: MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMeters(500)));
            }
        }
        catch (Exception ex)
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
}