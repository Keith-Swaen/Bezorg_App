using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Bezorg_App.Views;

namespace Bezorg_App.Views
{
    public partial class MainPage : ContentPage
    {
        private bool _isNavigating = false;
        private static readonly System.Globalization.CultureInfo _culture = System.Globalization.CultureInfo.InvariantCulture;

        public MainPage()
        {
            Preferences.Set("PrivacyConsent", false);

            InitializeComponent();
            CheckPrivacyConsent();
            TestApiKey();
        }

        private async void CheckPrivacyConsent()
        {
            // Controleer of de gebruiker al akkoord is gegaan
            bool hasAccepted = Preferences.Get("PrivacyConsent", false);
            if (!hasAccepted)
            {
                // Toon de privacy-popup
                await ShowPrivacyPopup();
            }
        }

        private async Task ShowPrivacyPopup()
        {
            await DisplayAlert(
                "Privacyvoorwaarden",
                "We waarderen je privacy. De Bezorg App verzamelt en verwerkt je gegevens, zoals locatie en bezorginformatie, om een optimale service te bieden. Deze gegevens worden alleen gebruikt voor het uitvoeren van bezorgingen en worden niet gedeeld met derden zonder jouw toestemming. Door verder te gaan, ga je akkoord met ons privacybeleid.",
                "Ga verder"
            );

            // Sla keuze op na accepteren
            Preferences.Set("PrivacyConsent", true);
        }

        private async void TestApiKey()
        {
            var settings = MauiProgram.Services.GetRequiredService<IOptions<ApiSettings>>().Value;
            string apiBaseUrl = settings.ApiBaseUrl;
            string url = $"{apiBaseUrl}/api/DeliveryServices";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    await DisplayAlert("API Key Test", "Success:\n" + await response.Content.ReadAsStringAsync(), "OK");
                else
                    await DisplayAlert("API Key Test", "Failed: " + response.StatusCode, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("API Key Test", "Error: " + ex.Message, "OK");
            }
        }

        private async void OnNavigateClicked(object sender, EventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            await Navigation.PushAsync(new BevestigBezorging());

            _isNavigating = false;
        }

        private async void OnLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync()
                              ?? await Geolocation.GetLocationAsync(new GeolocationRequest
                              {
                                  DesiredAccuracy = GeolocationAccuracy.Medium,
                                  Timeout = TimeSpan.FromSeconds(10)
                              });

                if (location != null)
                    await DisplayAlert("GPS Locatie", $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}", "OK");
                else
                    await DisplayAlert("Fout", "Kon locatie niet bepalen.", "OK");
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
                var location = await Geolocation.GetLastKnownLocationAsync()
                              ?? await Geolocation.GetLocationAsync(new GeolocationRequest
                              {
                                  DesiredAccuracy = GeolocationAccuracy.Medium,
                                  Timeout = TimeSpan.FromSeconds(10)
                              });

                if (location != null)
                {
                    var lat = location.Latitude.ToString(_culture);
                    var lon = location.Longitude.ToString(_culture);
                    var url = $"https://www.google.com/maps/search/?api=1&query={lat},{lon}";
                    await Launcher.Default.OpenAsync(url);
                }
                else
                    await DisplayAlert("Fout", "Kon locatie niet bepalen.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
            }
        }

        private async void OnViewStatusesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BezorgstatussenPage());
        }
        
        private async void OnKloktijdenClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KloktijdenPage());
        }
    }
}
