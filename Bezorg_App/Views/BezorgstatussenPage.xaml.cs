using Bezorg_App.Models;
using Bezorg_App.Models.Enums;
using Bezorg_App.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bezorg_App.Views
{
    public partial class BezorgstatussenPage : ContentPage
    {
        // Volledige lijst met bezorgstatussen en paginering
        private ObservableCollection<DeliveryState> _allStates = new();
        private const int PageSize = 5;
        private int _currentPage = 0;

        // Lijst met huidige pagina's bezorgstatussen
        public ObservableCollection<DeliveryState> CurrentPageStates { get; set; } = new();

        // Initaliseren overige eigenschappen en commando's
        public bool IsRefreshing { get; set; }
        public Command RefreshCommand { get; }
        public Command<DeliveryState> StateTappedCommand { get; }
        public Command<string> AddressTappedCommand { get; }
        public string PageIndicator => $"Pagina {_currentPage + 1} van {TotalPages}";
        private int TotalPages => (_allStates.Count + PageSize - 1) / PageSize;

        // Constructor
        public BezorgstatussenPage()
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
                string actie = await DisplayActionSheet($"Update status voor Order {state.OrderId}", "Annuleer", null,
                    "In Afwachting", "Onderweg", "Geleverd", "Geannuleerd");

                if (actie is null or "Annuleer")
                    return;

                try
                {
                    var service = MauiProgram.Services.GetRequiredService<IDeliveryStateService>();
                    DeliveryState updated;

                    switch (actie)
                    {
                        case "In Afwachting":
                            state.State = DeliveryStateEnum.InAfwachting;
                            state.DateTime = DateTime.UtcNow;
                            updated = await service.UpdateAsync(state);
                            break;
                        case "Onderweg":
                            state.State = DeliveryStateEnum.Onderweg;
                            state.DateTime = DateTime.UtcNow;
                            updated = await service.UpdateAsync(state);
                            break;
                        case "Geleverd":
                            state.State = DeliveryStateEnum.Geleverd;
                            state.DateTime = DateTime.UtcNow;
                            updated = await service.UpdateAsync(state);
                            break;
                        case "Geannuleerd":
                            state.State = DeliveryStateEnum.Geannuleerd;
                            state.DateTime = DateTime.UtcNow;
                            updated = await service.UpdateAsync(state);
                            break;
                        default:
                            return;
                    }

                    //Old
                    // Update lokaal
                    // int index = _allStates.IndexOf(state);
                    // if (index >= 0)
                    //     _allStates[index] = updated;

                    //New, update ui wanneer state veranderd
                    if (updated is not null)
                    {
                        state.State = updated.State;
                        state.DateTime = updated.DateTime;
                    }



                    await DisplayAlert("Bijgewerkt", $"Nieuwe status: {updated.State}", "OK");
                    LoadCurrentPage();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout bij update", ex.Message, "OK");
                }
            });

            AddressTappedCommand = new Command<string>(async (address) =>
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    await DisplayAlert("Fout", "Geen adres beschikbaar", "OK");
                    return;
                }

                try
                {
                    // Maak een Google Maps URL met het adres
                    var encodedAddress = Uri.EscapeDataString(address);
                    var mapsUrl = $"https://maps.google.com/maps?q={encodedAddress}";
                    
                    // Open Google Maps
                    await Launcher.OpenAsync(new Uri(mapsUrl));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout", $"Kon Google Maps niet openen: {ex.Message}", "OK");
                }
            });
        }
           
        // Laad de pagina met bezorgstatussen bij het openen
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatesAsync();
        }

        // Laad alle bezorgstatussen bij het openen van de pagina
        private async Task LoadStatesAsync()
        {
            try
            {
                var service = MauiProgram.Services.GetRequiredService<IDeliveryStateService>();
                var result = await service.GetAllAsync();
                _allStates = new ObservableCollection<DeliveryState>(
                    result.OrderByDescending(s => s.DateTime));
                _currentPage = 0;
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($">> EXCEPTION in LoadStatesAsync: {ex}");
                await DisplayAlert("Fout bij ophalen bezorgstatussen", ex.Message, "OK");
            }
        }

        // Laad de huidige pagina met bezorgstatussen
        private void LoadCurrentPage()
        {
            CurrentPageStates.Clear();
            var pageItems = _allStates.Skip(_currentPage * PageSize).Take(PageSize);
            foreach (var item in pageItems)
            {
                CurrentPageStates.Add(item);
            }

            OnPropertyChanged(nameof(PageIndicator));
        }

        // Knop om naar volgende pagina te gaan
        private void OnNextPageClicked(object sender, EventArgs e)
        {
            if (_currentPage < TotalPages - 1)
            {
                _currentPage++;
                LoadCurrentPage();
            }
        }

        // Knop om naar vorige pagina te gaan
        private void OnPreviousPageClicked(object sender, EventArgs e)
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                LoadCurrentPage();
            }
        }
    }
}
