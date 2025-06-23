using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bezorg_App.Models;
using Bezorg_App.Services;
using Microsoft.Extensions.Options;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Bezorg_App
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private bool _isRefreshing;
        private bool _isNavigating = false;
        private static readonly System.Globalization.CultureInfo _culture = System.Globalization.CultureInfo.InvariantCulture;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    OnPropertyChanged(nameof(IsRefreshing));
                }
            }
        }

        public Command RefreshCommand { get; }
        public Command<DeliveryState> StateTappedCommand { get; }

        private ObservableCollection<DeliveryState> _deliveryStates = new ObservableCollection<DeliveryState>();

        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            RefreshCommand = new Command(async () =>
            {
                IsRefreshing = true;
                await LoadStatesAsync();
                IsRefreshing = false;
            });

            StateTappedCommand = new Command<DeliveryState>(async (state) =>
            {
                string actie = await DisplayActionSheet(
                    $"Update status voor Order {state.OrderId}",
                    "Annuleer",
                    null,
                    "In afwachting",
                    "Onderweg",
                    "Afgeleverd",
                    "Geannuleerd");

                if (actie is null or "Annuleer")
                    return;

                state.State = actie switch
                {
                    "In afwachting" => DeliveryStateEnum.Pending,
                    "Onderweg" => DeliveryStateEnum.InProgress,
                    "Afgeleverd" => DeliveryStateEnum.Completed,
                    "Geannuleerd" => DeliveryStateEnum.Cancelled,
                    _ => state.State
                };

                state.DateTime = DateTime.UtcNow;

                try
                {
                    var service = MauiProgram.Services.GetRequiredService<IDeliveryStateService>();
                    var updated = await service.UpdateAsync(state);
                    await DisplayAlert("Status bijgewerkt", $"Nieuwe status: {updated.State}", "OK");

                    // Update lijst
                    int index = _deliveryStates.IndexOf(state);
                    if (index >= 0)
                    {
                        _deliveryStates[index] = updated;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout bij update", ex.Message, "OK");
                }
            });

            TestApiKey();
        }

        private async void TestApiKey()
        {
            var settings = MauiProgram.Services.GetRequiredService<IOptions<ApiSettings>>().Value;
            string apiKey = settings.DeliveryApiKey;
            string url = $"http://51.137.100.120:5000/api/DeliveryServices/{apiKey}";

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

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterBtn.Text = count == 1 ? $"Clicked {count} time" : $"Clicked {count} times";
            SemanticScreenReader.Announce(CounterBtn.Text);
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatesAsync();
        }

        private async Task LoadStatesAsync()
        {
            try
            {
                var service = MauiProgram.Services.GetRequiredService<IDeliveryStateService>();
                var allStates = await service.GetAllAsync();

                _deliveryStates.Clear();
                foreach (var state in allStates.OrderByDescending(s => s.DateTime).Take(10))
                {
                    _deliveryStates.Add(state);
                }

                StatesCollectionView.ItemsSource = _deliveryStates;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout bij ophalen bezorgstatussen", ex.ToString(), "OK");
            }
        }

        private async void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            if (sender is not Picker picker || picker.BindingContext is not DeliveryState state)
                return;

            try
            {
                state.DateTime = DateTime.UtcNow;
                var service = MauiProgram.Services.GetRequiredService<IDeliveryStateService>();
                var updated = await service.UpdateAsync(state);
                await DisplayAlert("Bijgewerkt", $"Order {updated.OrderId} staat nu op {updated.State}.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout bij update", ex.Message, "OK");
            }
        }
    }
}
