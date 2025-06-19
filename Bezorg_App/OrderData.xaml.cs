using System.Net.Http.Headers;
using System.Text.Json;

namespace Bezorg_App;

public partial class OrderData : ContentPage
{
    private const string ApiUrl = "https://51.137.100.120:5001/api/DeliveryServices";
    private const string ApiKey = "bbc3a1e6-98b8-42ce-98c3-0678bc59057a";

    public OrderData()
    {
        InitializeComponent();
        LoadOrderData();
    }

    private async void LoadOrderData()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ApiKey", ApiKey);

            var response = await client.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // OPTIONAL: Pretty print JSON
                var doc = JsonDocument.Parse(json);
                var formatted = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });

                ResultLabel.Text = formatted;
            }
            else
            {
                ResultLabel.Text = $"Fout bij ophalen: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            ResultLabel.Text = $"Fout: {ex.Message}";
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}
