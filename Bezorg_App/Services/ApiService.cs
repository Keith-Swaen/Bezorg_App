using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Bezorg_App.Models;
using Bezorg_App.Models.Enums;

namespace Bezorg_App.Services
{
    public class ApiService
    {
        // HttpClient wordt gebruikt om HTTP-verzoeken te doen naar de API
        private readonly HttpClient _httpClient;

        public ApiService(IOptions<ApiSettings> options)
        {
            var settings = options.Value;
            string apiBaseUrl = settings.ApiBaseUrl;
            // Initialiseer de HttpClient met de basis-URL van de API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl) // plaats hier je eigen ip
            };

            _httpClient.DefaultRequestHeaders.Add("apiKey", settings.DeliveryApiKey);
        }
        // Geef de HttpClient terug zodat deze hergebruikt kan worden in andere klassen 
        public HttpClient GetHttpClient() => _httpClient;

        // Customer service methoden
        public async Task<CustomerOrdersWithStatusResponse> GetCustomerOrdersWithStatusAsync(int customerId)
        {
            var response = await _httpClient.GetAsync($"api/Customer/{customerId}/orders");
            
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"API gaf status {(int)response.StatusCode} ({response.StatusCode}). Body:\n{body}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                // Eenvoudige JSON parsing zonder nullability context
                var result = ParseCustomerOrdersResponse(responseContent);
                return result ?? throw new ApplicationException("Geen data in response");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Kon response niet parsen: {ex.Message}");
            }
        }

        private CustomerOrdersWithStatusResponse ParseCustomerOrdersResponse(string json)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                var response = new CustomerOrdersWithStatusResponse();

                // Parse customer
                if (root.TryGetProperty("customer", out var customerElement))
                {
                    response.Customer = new Customer
                    {
                        Id = customerElement.GetProperty("id").GetInt32(),
                        Name = customerElement.GetProperty("name").GetString() ?? "",
                        Address = customerElement.GetProperty("address").GetString() ?? "",
                        Active = customerElement.GetProperty("active").GetBoolean()
                    };
                }

                // Parse ordersWithStatus
                if (root.TryGetProperty("ordersWithStatus", out var ordersElement))
                {
                    var ordersList = new List<OrderWithDeliveryStatus>();
                    
                    foreach (var orderElement in ordersElement.EnumerateArray())
                    {
                        var orderWithStatus = new OrderWithDeliveryStatus();

                        // Parse order
                        if (orderElement.TryGetProperty("order", out var orderObj))
                        {
                            orderWithStatus.Order = new Order
                            {
                                Id = orderObj.GetProperty("id").GetInt32(),
                                OrderDate = orderObj.GetProperty("orderDate").GetDateTime(),
                                CustomerId = orderObj.GetProperty("customerId").GetInt32(),
                                Customer = response.Customer
                            };
                        }

                        // Parse currentStatus
                        if (orderElement.TryGetProperty("currentStatus", out var statusElement))
                        {
                            orderWithStatus.CurrentStatus = (DeliveryStateEnum)statusElement.GetInt32();
                        }

                        // Parse deliveryHistory
                        if (orderElement.TryGetProperty("deliveryHistory", out var historyElement))
                        {
                            var deliveryStates = new List<DeliveryState>();
                            
                            foreach (var stateElement in historyElement.EnumerateArray())
                            {
                                var deliveryState = new DeliveryState
                                {
                                    Id = stateElement.GetProperty("id").GetInt32(),
                                    State = (DeliveryStateEnum)stateElement.GetProperty("state").GetInt32(),
                                    DateTime = stateElement.GetProperty("dateTime").GetDateTime(),
                                    OrderId = stateElement.GetProperty("orderId").GetInt32(),
                                    Order = orderWithStatus.Order
                                };

                                if (stateElement.TryGetProperty("deliveryServiceId", out var serviceIdElement))
                                {
                                    deliveryState.DeliveryServiceId = serviceIdElement.ValueKind == JsonValueKind.Null ? null : serviceIdElement.GetInt32();
                                }

                                deliveryStates.Add(deliveryState);
                            }

                            orderWithStatus.DeliveryHistory = deliveryStates;
                        }

                        // Parse lastUpdated
                        if (orderElement.TryGetProperty("lastUpdated", out var lastUpdatedElement))
                        {
                            orderWithStatus.LastUpdated = lastUpdatedElement.GetDateTime();
                        }

                        ordersList.Add(orderWithStatus);
                    }

                    response.OrdersWithStatus = ordersList;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Kon response niet parsen: {ex.Message}");
            }
        }
    }
}
