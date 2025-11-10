using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TickAndDash.HttpClients.GeocodingClient.DTOs;
using TickAndDash.HttpClients.Interfaces;

namespace TickAndDash.HttpClients.GeoClients
{
    public class GeocodingClient : IGeocodingClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _aPIKey;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource =
         new CancellationTokenSource();
        public GeocodingClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri("https://api.geoapify.com");
            _httpClient.Timeout = new TimeSpan(0, 0, 60);
            _httpClient.DefaultRequestHeaders.Clear();
            _aPIKey = _configuration.GetValue<string>("GeoapifyOptions:APIKey");
        }

        public async Task<APIConsumingResponse<string>> GetPlaceAddressForCoordinates(decimal lat, decimal lng)
        {
            var aPIConsumingResponse = new APIConsumingResponse<string>
            {
                ThirdPartyStatusCode = System.Net.HttpStatusCode.UnprocessableEntity
            };

            var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/geocode/reverse?lat={lat}&lon={lng}&lang=de&limit=1&apiKey={_aPIKey}");


            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);

            var responseStream = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var reverseGeocode = await JsonSerializer.DeserializeAsync<ReverseGeocode>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    aPIConsumingResponse.ThirdPartyStatusCode = System.Net.HttpStatusCode.OK;
                    aPIConsumingResponse.IsDeserializeSuccess = true;
                    aPIConsumingResponse.ThirdPartyResponse = reverseGeocode?.Features?.FirstOrDefault()?.Properties?.City;
                }
                catch (Exception ex)
                {
                    aPIConsumingResponse.IsDeserializeSuccess = false;
                }
            }



            return aPIConsumingResponse;
        }
    }
}
