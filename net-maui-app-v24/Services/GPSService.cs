namespace net_maui_app_v24.Services
{
    public class GPSService
    {
        public Location location { get; set; }
        public async Task<string> GetCurrentLocation()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10));
                Location location = await Geolocation.GetLocationAsync(request);
                this.location = location;

                if (location != null)
                {
                    return $"Latitude: {location.Latitude}, Longitude: {location.Longitude}";
                }
                else
                {
                    return "Não foi possível obter a localização.";
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }
    }
}
