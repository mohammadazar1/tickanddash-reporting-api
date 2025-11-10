using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TickAndDash.Extensions;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.HttpClients.GeoClients.Interfaces;
using TickAndDash.HttpClients.GeocodingClient.DTOs;

namespace TickAndDash.HttpClients
{
    public class DigitalCodexClient : IDigitalCodexClient
    {

        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public DigitalCodexClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://192.168.100.68:1000/");
            //_httpClient.BaseAddress = new Uri("http://localhost:45000/");
            _httpClient.Timeout = new TimeSpan(0, 0, 60);
            _httpClient.DefaultRequestHeaders.Add("ApiKey", "8giMAgy54kZWQwMyLVnCjp43n3SqjWJ6QoKy3YZ3FKY=");
        }

        public async Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync(RegisterUserDto registerUser)
        {

            DigitalCodexResponseDto<DigitalCodexRegisterUserResponse> digitalCodexResponseDto =
                new DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>();

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Users/services/register");
            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(new
            {
                registerUser.CanUseBalance,
                registerUser.CountryId,
                registerUser.Email,
                registerUser.FlowId,
                registerUser.Location,
                registerUser.Name,
                registerUser.Password,
                registerUser.UserName,
                registerUser.MSISDN,
            });

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var streamContent = new StreamContent(memoryContentStream))
            {   
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                //var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();
                //if (response.IsSuccessStatusCode)
                //{
                digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                Log.
                  ForContext("registerUserRequest", registerUser, true).
                  ForContext("registerUserResponse", digitalCodexResponseDto, true).
                  Information("DC_RegisterUserAsync()");

                //}
                return digitalCodexResponseDto;
            }
        }

        public async Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token, string lang)
        {
            DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>> digitalCodexResponseDto = new DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>();

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/balance/services/get-balance?lang={lang}");
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
        public async Task<DigitalCodexResponseDto<object>> ReserveBalanceAsync(ReserveBalanceRequest reserveBalanceRequest)
        {
            DigitalCodexResponseDto<object> digitalCodexResponseDto = new DigitalCodexResponseDto<object>();
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/balance/services/reserve");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", reserveBalanceRequest.Token);

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(new
            {
                reserveBalanceRequest.ReservationBalance,
                reserveBalanceRequest.ReservationCurrency,
                reserveBalanceRequest.ReservationPeriod
            });

            memoryContentStream.Seek(0, SeekOrigin.Begin);
            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();
                digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                return digitalCodexResponseDto;
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
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();


                //if (response.IsSuccessStatusCode)
                //{
                responseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                //}

                Log.
                  ForContext("TransferBalanceRequest", transferBalance, true).
                  ForContext("TransferBalanceResponse", responseDto, true).
                  Information("DC_TransferBalanceAsync()");


                return responseDto;
            }
        }

        public async Task<DigitalCodexResponseDto<object>> CancelReservationAsync(CancelReservationRequest cancelReservation)
        {
            DigitalCodexResponseDto<object> digitalCodexResponseDto = new DigitalCodexResponseDto<object>();

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Balance/services/cancel-reservation");
            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(new
            {
                cancelReservation.ReservationCurrency
            }
            );

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                //var responseStream = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();

                digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                Log.
                    ForContext("CancelReservationRequest", cancelReservation, true).
                    ForContext("CancelReservationResponse", digitalCodexResponseDto, true).
                    Information("DC_CancelReservation()");

                return digitalCodexResponseDto;
            }
        }
        public async Task<DigitalCodexResponseDto<object>> SubscribeAysnc(SubscribeRequest subscribeRequest)
        {
            DigitalCodexResponseDto<object> digitalCodexResponseDto = new DigitalCodexResponseDto<object>();
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Subscriptions/sub");
            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(
                new
                {
                    serviceId = subscribeRequest.ServiceId,
                    language = subscribeRequest.Language
                }
            );

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent; request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", subscribeRequest.Token);

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();

                digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                Log.
                    ForContext("SubscribeRequest", subscribeRequest, true).
                    ForContext("SubscribeResponse", digitalCodexResponseDto, true).
                    Information("DC_SubscribeRequest()");

                return digitalCodexResponseDto;
            }
        }

        public async Task<DigitalCodexResponseDto<object>> UnSubscribeAysnc(SubscribeRequest unSubscribeRequest)
        {
            DigitalCodexResponseDto<object> digitalCodexResponseDto = new DigitalCodexResponseDto<object>();

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Subscriptions/unsub");
            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(
                new
                {
                    serviceId = unSubscribeRequest.ServiceId
                }
            );


            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", unSubscribeRequest.Token);
            memoryContentStream.Seek(0, SeekOrigin.Begin);
            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseStream = await response.Content.ReadAsStreamAsync();

                digitalCodexResponseDto = await JsonSerializer.DeserializeAsync<DigitalCodexResponseDto<object>>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                
                Log.
                    ForContext("unSubscribeRequest", unSubscribeRequest, true).
                    ForContext("unSubscribeRequest", digitalCodexResponseDto, true).
                    Information("DC_UnSubscribeAysnc");

                return digitalCodexResponseDto;
            }
        }
    }
}
