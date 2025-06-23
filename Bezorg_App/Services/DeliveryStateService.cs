using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bezorg_App.Models;

namespace Bezorg_App.Services
{
    /// <summary>
    /// Roept de API aan om alle DeliveryStates op te halen.
    /// </summary>
    public class DeliveryStateService : IDeliveryStateService
    {
        private readonly HttpClient _httpClient;

        public DeliveryStateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IList<DeliveryState>> GetAllAsync()
        {
            // expliciet de volledige URL aanroepen voor duidelijkheid
            var requestUri = "api/DeliveryStates/GetAllDeliveryStates";
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(requestUri);
            }
            catch (Exception httpEx)
            {
                // network/TLS/DNS errors
                throw new ApplicationException(
                    $"Netwerkfout bij GET {requestUri}: {httpEx.Message}", httpEx);
            }

            if (!response.IsSuccessStatusCode)
            {
                // krijg je 404 / 500 / 401 / etc
                var body = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"API gaf status {(int)response.StatusCode} ({response.StatusCode}). Body:\n{body}");
            }

            try
            {
                var result = await response.Content
                    .ReadFromJsonAsync<IList<DeliveryState>>();
                return result ?? Array.Empty<DeliveryState>();
            }
            catch (Exception jsonEx)
            {
                // JSON-deserialisatieproblemen
                var raw = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"Kon JSON niet deserialiseren:\n{jsonEx.Message}\nRAW:\n{raw}", jsonEx);
            }
        }
    }
}
