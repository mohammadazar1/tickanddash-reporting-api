using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TickAndDashReportingTool.Ext;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;

namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class DigitalCodexClient : IDigitalCodexClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public DigitalCodexClient(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://192.168.100.68:1000/");
            _httpClient.Timeout = new TimeSpan(0, 0, 60);

            _httpClient.DefaultRequestHeaders.Add("ApiKey", _configuration["DigitalCodexApiKey"]);
        }

        public async Task<DigitalCodexResponseDto<object>> ConsumeFromDriverAsync(string mobileNumber, double paymentAmount, string currancyType)
        {
            DigitalCodexResponseDto<object> digitalCodexResponseDto =
                new DigitalCodexResponseDto<object>();

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Wallets/Consume");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["DigitalCodexToken"]);

            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(new
            {
                Msisdn = "972" + mobileNumber.Substring(mobileNumber.Length - 9),
                CurrencyCode = currancyType,
                Amount = paymentAmount,
            });

            memoryContentStream.Seek(0, SeekOrigin.Begin);
            using (var streamContent = new StreamContent(memoryContentStream))
            {

                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();
                //if (response.IsSuccessStatusCode)
                //{
                digitalCodexResponseDto = await JsonSerializer
                    .DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                //}

                return digitalCodexResponseDto;
            }
        }

        public async Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync(RegisterUserDto registerUser)
        {
            DigitalCodexResponseDto<DigitalCodexRegisterUserResponse> digitalCodexResponseDto =
                new DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>();

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["DigitalCodexToken"]);

            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(new
            {
                registerUser.CanUseBalance,
                registerUser.CountryId,
                registerUser.Email,
                //registerUser.FlowId,
                registerUser.Location,
                registerUser.Name,
                registerUser.Password,
                registerUser.UserName,
                registerUser.MSISDN,
                registerUser.UserType
            });

            memoryContentStream.Seek(0, SeekOrigin.Begin);
            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();
                //if (response.IsSuccessStatusCode)
                //{
                try
                {
                    digitalCodexResponseDto = await JsonSerializer
                                     .DeserializeAsync<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>>(responseStream, new JsonSerializerOptions
                                     {
                                         PropertyNameCaseInsensitive = true,
                                     });
                    //}
                    return digitalCodexResponseDto;

                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }

        public async Task<DigitalCodexResponseDto<object>> TransferBalanceAsync(TransferBalanceRequest transferBalance)
        {
            DigitalCodexResponseDto<object> responseDto = new DigitalCodexResponseDto<object>();
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/balance/services/transfer");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", transferBalance.Token);

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(new
            {
                transferBalance.MobileNumber,
                transferBalance.TransferBalance,
                TransferCurrency = "ILS",
                transferBalance.Username
            });

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseStr = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();

                //if (response.IsSuccessStatusCode)
                //{
                responseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                //}
                return responseDto;
            }
        }

        public async Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token)
        {
            DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>> digitalCodexResponseDto = new DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>();

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/balance/services/get-balance?lang=ar");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseStream = await response.Content.ReadAsStreamAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>>
                  (responseStream, new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true,
                  });
                }
                catch (Exception ex)
                {

                }

            }
            //else
            //{
            //    var x = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>>(responseStream, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true,
            //    });
            //}

            return digitalCodexResponseDto;
        }
    }
}


