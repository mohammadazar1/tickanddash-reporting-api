using Serilog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDash.Services.ServicesDtos;

namespace TickAndDash.Services
{
    public class BulkSMSService : IBulkSMSService
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public BulkSMSService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            string baseUrl = "http://192.168.100.36:8800/ ";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task<bool> SendSMSAsync(SMSDto sMSDto)
        {
            //var request = new HttpRequestMessage(HttpMethod.Post, $"?user=TickDash&password=yKB%8v&PhoneNumber=972{sMSDto.MSISDN.Substring(sMSDto.MSISDN.Length - 9)}&Text={sMSDto.Msg}&Sender=TickDash");

            var request = new HttpRequestMessage(HttpMethod.Post, $"?user=TickDash&password=123456&PhoneNumber=972{sMSDto.MSISDN.Substring(sMSDto.MSISDN.Length - 9)}&Text={sMSDto.Msg}&Sender=TickDash");

            /// example
            /// http://192.168.100.36:8800/?user=@@Anas@@&password=@@123456@@&PhoneNumber=@@972598660503Text=test&Sender=Sender=WAFANetwork
            ///

            bool isSuccess = false;
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (responseString != null)
                {
                    if (responseString.Contains("Message Submitted"))
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        Log.
                            ForContext("BulkSMSResponse", responseString).
                            Fatal("Error in Bulk SMS Response");
                        //
                        //log warning 
                    }
                }
                else
                {
                    //log  error
                }
            }
            else
            {
                if (responseString.Contains("credit limit exceeded"))
                {
                    Log.
                           ForContext("BulkSMSResponse", responseString).
                           Fatal("No Credit in Bulk SMS Response");
                    // log error that balance is off for this account and need to refill it
                }
                else
                {
                    Log.
                           ForContext("BulkSMSResponse", responseString).
                           Fatal("Error in Bulk SMS Response");
                }
            }

            return isSuccess;
        }

    }
}
