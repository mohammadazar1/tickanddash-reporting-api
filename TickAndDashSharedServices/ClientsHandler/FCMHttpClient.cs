using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TickAndDashSharedServices.ClientsHandler.Dtos;

namespace TickAndDashSharedServices.ClientsHandler.Interfaces
{
    public class FCMHttpClient : IFCMHttpClient
    {
        //private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource =
          new CancellationTokenSource();
        private string FcmKey = "AAAAo_oCIiA:APA91bHPyaRZWWe50ymtnAEVP3a1_QXzf4L9pgN_rw_p0Pnr_LCBhty2cx3UjLWWKf-EJD2-isyvCVCUpdwoEa6F6ZDNzx-NJFX00yDj5UlHf1on_smLt66zdx6yoUXyQIgfPoefoLGM";

        public FCMHttpClient(/*HttpClient httpClient*/)
        {
            //_httpClient = new HttpClient();// httpClient; 
            //_httpClient.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
            //_httpClient.Timeout = new TimeSpan(0, 0, 5);
            //_httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> PushNotificationsAsync(PushNotificationDto pushNotificationDto)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + FcmKey);
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "send"))
                    {
                        var json = JsonConvert.SerializeObject(pushNotificationDto);
                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            request.Content = stringContent;
                            using (var response = await client
                                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                .ConfigureAwait(false))
                            {
                                string resp = await response.Content.ReadAsStringAsync();
                                var fcmPushResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FcmPushResponse>(resp);
                                return fcmPushResponse.Success > 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                // Log exception
            }

            //var _httpClient = new HttpClient();
            //_httpClient.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
            //_httpClient.Timeout = new TimeSpan(0, 0, 5);
            //_httpClient.DefaultRequestHeaders.Clear();



            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "send");
            //var memoryContentStream = new MemoryStream();
            //memoryContentStream.SerializeToJsonAndWrite(pushNotificationDto);
            //memoryContentStream.Seek(0, SeekOrigin.Begin);
            //using (var streamContent = new StreamContent(memoryContentStream))
            //{
            //    request.Content = streamContent;
            //    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + FcmKey);

            //    try
            //    {
            //        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, _cancellationTokenSource.Token);
            //        //var responseStream = await response.Content.ReadAsStringAsync();
            //        if (response.IsSuccessStatusCode)
            //        {
            //            //var res = await response.Content.ReadAsStringAsync();
            //            var responseStream = await response.Content.ReadAsStreamAsync();
            //            var pushResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<FcmPushResponse>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //            if (pushResponse.Success == 1)
            //            {
            //                return true;
            //            }
            //        }
            //        else
            //        {
            //            var responseStream = await response.Content.ReadAsStringAsync();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}

            return false;
        }
    }
}
