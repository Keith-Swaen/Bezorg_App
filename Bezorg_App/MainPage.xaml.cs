using System;
using System.Net.Http;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Options;
using Bezorg_App.Models;
using Bezorg_App.Services;

namespace Bezorg_App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            TestApiKey();
        }

        private async void TestApiKey()
        {
            // Haal de API-sleutel op uit de configuratie via dependency injection
            var settings = MauiProgram.Services
                                       .GetRequiredService<IOptions<ApiSettings>>()
                                       .Value;
            string apiKey = settings.DeliveryApiKey;

            string url = $"http://51.137.100.120:5000/api/DeliveryServices/{apiKey}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("API Key Test", "Success:\n" + json, "OK");
                }
                else
                {
                    await DisplayAlert("API Key Test", "Failed: " + response.StatusCode, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("API Key Test", "Error: " + ex.Message, "OK");
            }
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
                    await DisplayAlert(
                        "GPS Locatie",
                        $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}",
                        "OK");
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

        private async void OnOpenInMapsClicked(object sender, EventArgs e)
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
                    var lat = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var lon = location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var url = $"https://www.google.com/maps/search/?api=1&query={lat},{lon}";
                    await Launcher.Default.OpenAsync(url);
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var service = MauiProgram.Services
                                  .GetRequiredService<IDeliveryStateService>();
                var states = await service.GetAllAsync();
                StatesCollectionView.ItemsSource = states;
            }
            catch (Exception ex)
            {
                // toont volledige stack en inner exceptions
                await DisplayAlert(
                    "Fout bij ophalen bezorgstatussen",
                    ex.ToString(),
                    "OK");
            }
        }
    }
}
