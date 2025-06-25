using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

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
    }
}
