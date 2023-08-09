namespace UTeM_Mobile.Services
{
    public class LocationService
    {
        public static async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Granted)
                {
                    CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
                    return await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
                }
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
