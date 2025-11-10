using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TickAndDash.ClientsHandler.Dtos;
using TickAndDash.ClientsHandler.Interfaces;
using TickAndDash.Extensions;

namespace TickAndDash.ClientsHandler
{
    public class FCMHttpClient : IFCMHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource =
          new CancellationTokenSource();
        private string FcmKey = "AAAAo_oCIiA:APA91bHPyaRZWWe50ymtnAEVP3a1_QXzf4L9pgN_rw_p0Pnr_LCBhty2cx3UjLWWKf-EJD2-isyvCVCUpdwoEa6F6ZDNzx-NJFX00yDj5UlHf1on_smLt66zdx6yoUXyQIgfPoefoLGM";

        public FCMHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.Authorization =
            //     new AuthenticationHeaderValue("key", "=" +  FcmKey);
        }

        public async Task<bool> PushNotificationsAsync( PushNotificationDto pushNotificationDto)
        {

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");

            //client.DefaultRequestHeaders
            //          .Accept
            //          .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + FcmKey);
            ////client.DefaultRequestHeaders.Add("Sender", "id=" + fcmDetails.PROJECT_KEY)
            //var data = new
            //{
            //    to = FCMToken,
            //    notification = new
            //    {
            //        body = "This is the message",
            //        title = "This is the title",
            //        icon = "myicon"
            //    }
            //};

            //var json = JsonSerializer.Serialize(data);
            //request.Content = new StringContent(json,
            //                                    Encoding.UTF8,
            //                                    "application/json");//CONTENT-TYPE header

            //var data1 = client.PostAsync("send", request.Content);
            //var d = data1.Result.Content.ReadAsStringAsync();

            //var request = new HttpRequestMessage(HttpMethod.Post, $"fcm/send");
            //pushNotificationDto.to = FCMToken??"";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "send");
            var memoryContentStream = new MemoryStream();

            memoryContentStream.SerializeToJsonAndWrite(pushNotificationDto);

            //memoryContentStream.SerializeToJsonAndWrite(new {
            //    to = FCMToken,
            //    notification = new
            //    {
            //        body = "This is the message",
            //        title = "This is the title",
            //      icon = "myicon"
            //      }
            //});

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var streamContent = new StreamContent(memoryContentStream))
            {
                request.Content = streamContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + FcmKey);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, _cancellationTokenSource.Token);
                //var responseStream = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var responseStream = await response.Content.ReadAsStreamAsync();

                    var pushResponse = await JsonSerializer.DeserializeAsync<FcmPushResponse>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (pushResponse.Success == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                }
            }

            return false;
        }
    }
}

