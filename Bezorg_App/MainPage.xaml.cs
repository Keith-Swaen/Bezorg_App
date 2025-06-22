using Microsoft.Maui.ApplicationModel;

namespace Bezorg_App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnNavigateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BevestigBezorging());
        }

        private async void OnLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(10)
                    });
                }

                if (location != null)
                {
                    await DisplayAlert("GPS Locatie", $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}", "OK");
                }
                else
                {
                    await DisplayAlert("Fout", "Kon locatie niet bepalen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
            }
        }
    }
}
