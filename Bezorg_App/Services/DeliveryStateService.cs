using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bezorg_App.Models;
using Bezorg_App.Models.Enums;
using Microsoft.Extensions.Options;

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
            var apiService = new ApiService(MauiProgram.Services.GetRequiredService<IOptions<ApiSettings>>());
            var allDeliveryStates = new List<DeliveryState>();

            try
            {
                // Probeer customer 1 op te halen
                var customerId = 1;
                var customerOrdersResponse = await apiService.GetCustomerOrdersWithStatusAsync(customerId);
                
                if (customerOrdersResponse.OrdersWithStatus != null)
                {
                    foreach (var orderWithStatus in customerOrdersResponse.OrdersWithStatus)
                    {
                        if (orderWithStatus.DeliveryHistory != null)
                        {
                            allDeliveryStates.AddRange(orderWithStatus.DeliveryHistory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij ophalen delivery states: {ex.Message}");
            }

            return allDeliveryStates;
        }

        public async Task<DeliveryState> UpdateAsync(DeliveryState state)
        {
            HttpResponseMessage response;
            try
            {
                var updateRequest = new UpdateDeliveryStateRequest
                {
                    State = state.State,
                    DeliveryServiceId = state.DeliveryServiceId
                };

                response = await _httpClient.PutAsJsonAsync(
                    $"api/DeliveryStates/{state.Id}",
                    updateRequest);
            }
            catch (Exception httpEx)
            {
                throw new ApplicationException(
                    $"Netwerkfout bij PUT UpdateDeliveryState: {httpEx.Message}", httpEx);
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