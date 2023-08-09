using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using PusherClient;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.ViewModels.Supervisor;

namespace UTeM_Mobile.Views.Supervisor;

public partial class PatrolDetailPage : ContentPage
{
	private PatrolDetailViewModel viewModel;
    private Location location;
    private Location location1;
    private Location location2;
	public PatrolDetailPage()
	{
		InitializeComponent();
        viewModel = BindingContext as PatrolDetailViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
        await LocateGuardAsync();
    }

    public async Task GetCurrentLocation()
    {
        try
        {
            var checkpoint1 = viewModel.Patrol.Route.RouteCheckpoints.FirstOrDefault().Checkpoint;
            var checkpoint2 = viewModel.Patrol.Route.RouteCheckpoints.LastOrDefault().Checkpoint;
            location1 = new Location(checkpoint1.Latitude, checkpoint1.Longitude);
            location2 = new Location(checkpoint2.Latitude, checkpoint2.Longitude);
            location = await LocationService.GetCurrentLocationAsync();
            if (location != null && location1 != null && location2 != null && viewModel.ShowGPS)
            {
                map.MoveToRegion(mapSpan: MapSpan.FromCenterAndRadius(location, Distance.FromMeters(500)));
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

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ((ListView)sender).SelectedItem = null;
        ((ListView)sender).BackgroundColor = Colors.Transparent;
    }

    private async Task LocateGuardAsync()
    {
        Pusher pusher = new Pusher(PusherCredential.key, new PusherOptions
        {
            Cluster = PusherCredential.cluster,
            Encrypted = true
        });
        Channel PrivateChannel;
        try
        {
            await pusher.ConnectAsync().ConfigureAwait(false);
            PrivateChannel = await pusher.SubscribeAsync("UTeM-Guard").ConfigureAwait(false);
            PrivateChannel.Bind("guard.activities." + viewModel.Id, GuardLocationListener);
        }
        catch (Exception ex)
        {

        }
    }

    private void GuardLocationListener(object sender)
    {
        try
        {
            if (viewModel.IsBusy)
            {
                return;
            }
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(sender));
            if (dictionary.ContainsKey("data"))
            {
                PatrolLocation patrolLocation = JsonConvert.DeserializeObject<PatrolLocation>(dictionary["data"]);
                if (patrolLocation != null)
                {
                    location = patrolLocation.Location;
                    string guard = viewModel.Patrol != null && viewModel.Patrol.Guard != null && viewModel.Patrol.Guard.Name != null ? viewModel.Patrol.Guard.Name : "N/A";
                    Pin pin = new Pin
                    {
                        Label = guard,
                        Location = location
                    };
                    if (location != null)
                    {
                        if (viewModel.ShowGPS)
                        {
                            map.MoveToRegion(mapSpan: MapSpan.FromCenterAndRadius(location, Distance.FromMeters(300)));
                        }

                    }
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        map.Pins.Clear();
                        map.Pins.Add(pin);
                    });
                }
            }
            
        }
        catch (Exception ex)
        {

        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        viewModel.ShowMapRoute = !viewModel.ShowMapRoute;
        if (viewModel.ShowGPS)
        {
            await GetCurrentLocation();
        }
    }
}