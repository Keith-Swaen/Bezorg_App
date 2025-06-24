using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bezorg_App.Models;

namespace Bezorg_App.Services
{
    /// <summary>
    /// Roept de API aan om alle DeliveryStates op te halen en te updaten.
    /// </summary>
    public class DeliveryStateService : IDeliveryStateService
    {
        private readonly HttpClient _httpClient;

        public DeliveryStateService(ApiService apiService)
        {
            _httpClient = apiService.GetHttpClient();
        }

        public async Task<IList<DeliveryState>> GetAllAsync()
        {
            var requestUri = "api/DeliveryStates/GetAllDeliveryStates";
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(requestUri);
            }
            catch (Exception httpEx)
            {
                throw new ApplicationException(
                    $"Netwerkfout bij GET {requestUri}: {httpEx.Message}", httpEx);
            }

            if (!response.IsSuccessStatusCode)
            {
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
                var raw = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"Kon JSON niet deserialiseren:\n{jsonEx.Message}\nRAW:\n{raw}", jsonEx);
            }
        }

        public async Task<DeliveryState> UpdateAsync(DeliveryState state)
        {
            // Endpoint uit swagger: POST /api/DeliveryStates/UpdateDeliveryState
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsJsonAsync(
                    "api/DeliveryStates/UpdateDeliveryState",
                    state);
            }
            catch (Exception httpEx)
            {
                throw new ApplicationException(
                    $"Netwerkfout bij POST UpdateDeliveryState: {httpEx.Message}", httpEx);
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"Update faalde ({(int)response.StatusCode}): {body}");
            }

            try
            {
                var updated = await response.Content
                                           .ReadFromJsonAsync<DeliveryState>();
                return updated
                       ?? throw new ApplicationException("Geen state in response");
            }
            catch (Exception jsonEx)
            {
                var raw = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"Kon JSON niet deserialiseren na update:\n{jsonEx.Message}\nRAW:\n{raw}", jsonEx);
            }
        }
    }
}
