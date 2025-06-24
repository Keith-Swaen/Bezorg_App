using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bezorg_App.Models;
using Bezorg_App.Services;
using Microsoft.Maui.Controls;
using Bezorg_App.Models.Enums;

namespace Bezorg_App.Pages
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
                    "In afwachting", "Onderweg", "Afgeleverd", "Geannuleerd");

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

                    // Update lokaal
                    int index = _allStates.IndexOf(state);
                    if (index >= 0)
                        _allStates[index] = updated;

                    await DisplayAlert("Bijgewerkt", $"Nieuwe status: {updated.State}", "OK");
                    LoadCurrentPage();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout bij update", ex.Message, "OK");
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
