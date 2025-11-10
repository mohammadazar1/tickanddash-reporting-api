using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDash.Services.ServicesDtos;

namespace TickAndDash.Services
{
    public class SMSService : ISMSService
    {
        List<string> msisdns = new List<string>()
        {
            "0598660503",
            "0597072839",
            "0597888617",
            "0597072837",
            "+972568488035",
            "0592129159",
            "+972568916162",
            "+972569455584",
            "+972569384420",
            "0599000729",
            "0599001361",
            "0597422435",
            "+972569384420",
            "0592828525",
            "0594699902",
            "0599001361",
            "0597291219",
            "0599672670",
            "0597954365",
        };

        public async Task<bool> SendSMSToUserAsync(string msisdn, string msg)
        {
            if (msisdn.Substring(msisdn.Length - 9).StartsWith("59"))
            {
                msisdn = $"0{msisdn.Substring(msisdn.Length - 9)}";
            }
            else if (msisdn.Substring(msisdn.Length - 9).StartsWith("56"))
            {
                msisdn = $"+972{msisdn.Substring(msisdn.Length - 9)}";
            }

            //if (msisdns.IndexOf(msisdn) == -1)
            //{
            //    return false;
            //}
            //await SendNotificationMessage(msisdn, msg);
            InsertToSender(msisdn, msg);
            //try
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "http://192.168.100.39:8085/api/MW/DCB/SendSMS"))
            //        {

            //            var postValues = new Dictionary<string, string>
            //        {
            //           {"Message", msg},
            //           {"SenderName", "Upay"},
            //           {"MSISDN",  msisdn},
            //           {"Shortcode", "37637"},
            //           {"ServiceId", "5189"},
            //           {"SPId", "15"},
            //        };

            //            req.Content = new FormUrlEncodedContent(postValues);
            //            var response = await client.SendAsync(req);

            //            if (response.IsSuccessStatusCode)
            //            {
            //                return true;
            //            }
            //            //string res1 = "";
            //            //using (HttpContent content2 = response.Content)
            //            //{
            //            //    Task<string> result = content2.ReadAsStringAsync();
            //            //    res1 = result.Result;
            //            //}
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // _logger.Error("error sending the sms", ex);
            //}
            return true;
        }


        private async Task SendNotificationMessage(string msisdn, string message)
        {
            StringBuilder msgFileBuilder = new StringBuilder();
            string queuePath = "\\\\192.168.100.53\\Q\\";

            msisdn = msisdn.Substring(msisdn.Length - 9);
            try
            {
                if (msisdn.StartsWith("59"))
                {
                    //msisdn = "0" + msisdn;

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "http://192.168.100.39:8085/api/MW/DCB/SendSMS"))
                        {
                            var postValues = new Dictionary<string, string>
                            {
                               {"Message", message},
                               {"SenderName", "Upay"},
                               {"MSISDN",  $"972{msisdn}"},
                               {"Shortcode", "37637"},
                               {"ServiceId", "5189"},
                               {"SPId", "15"},
                            };

                            req.Content = new FormUrlEncodedContent(postValues);
                            var response = await client.SendAsync(req);

                            //string res1 = "";
                            //using (HttpContent content2 = response.Content)
                            //{
                            //    Task<string> result = content2.ReadAsStringAsync();
                            //    res1 = result.Result;
                            //}
                        }
                    }
                }
                else if (msisdn.StartsWith("56"))
                {
                    if (!message.Contains("Password"))
                    {
                        message = ArabicToHex(message);
                    }

                    msisdn = "972" + msisdn;

                    msgFileBuilder = new StringBuilder();
                    msgFileBuilder.AppendLine("[SMS]");
                    msgFileBuilder.AppendLine("SubmittedBy=127.0.0.1");
                    msgFileBuilder.AppendLine("PhoneNumber=" + msisdn);
                    msgFileBuilder.AppendLine("Data=" + message);
                    msgFileBuilder.AppendLine("pid=00");
                    if (message.Contains("Password"))
                    {
                        msgFileBuilder.AppendLine("dcs=00");
                        msgFileBuilder.AppendLine("Binary=0");
                    }
                    else
                    {
                        msgFileBuilder.AppendLine("dcs=08");
                        msgFileBuilder.AppendLine("Binary=1");
                    }
                    msgFileBuilder.AppendLine("Sender=Upay");

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@queuePath + "cash_" + msisdn + DateTime.Now.Millisecond + ".req"))
                    {
                        file.Write(msgFileBuilder.ToString());
                        file.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.Info($"[Livat@] Exception Happened when sending messages: _{ex.Message}");
            }
        }

        private string ArabicToHex(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                try
                {
                    sb.Append(string.Format("{0:X4}", Convert.ToInt32(c)));
                }
                catch
                {

                }
            }
            return sb.ToString();
        }

        private void InsertToSender(string msisdn, string msg)
        {
            OutRespons outRespons = new OutRespons
            {
                Msg = $"{msg}",
                MobileNumber = msisdn,
                CreationDate = DateTime.Now,
                Status = "DCB Notification",
            };

            string query = " INSERT INTO OutResponses" +
                " ( UserServiceId, Lang, ShortCode, IsHex, Msg, MobileNumber, SenderId, CreationDate, Sent, IsPIN, IsPinSent, Status, IsStatusDelivered, Operator) " +
                "  VALUES ( @UserServiceId, @Lang, @ShortCode, @IsHex, @Msg, @MobileNumber, @SenderId, @CreationDate, @Sent, @IsPIN, @IsPinSent, @Status, @IsStatusDelivered, @Operator)";
            //" SELECT SCOPE_IDENTITY() ";

            using (var sqlConnection = new SqlConnection("Data Source = .;Initial Catalog = DirectBillingDB; User ID = sa; Password=xxx"))
            {
                int affectedRows = sqlConnection.Execute(query, new
                {
                    UserServiceId = 0,
                    Lang = "ar",
                    ShortCode = outRespons.MobileNumber.StartsWith("+972") ? 7902 : 37637,
                    IsHex = 1,
                    Msg = outRespons.MobileNumber.StartsWith("+972") ? Helpers.ArabicToHex(outRespons.Msg) : outRespons.Msg,
                    outRespons.MobileNumber,
                    SenderId = 0,
                    CreationDate = DateTime.Now,
                    Sent = 0,
                    IsPIN = 0,
                    IsPinSent = 0,
                    outRespons.Status,
                    IsStatusDelivered = 0,
                    Operator = outRespons.MobileNumber.StartsWith("+972") ? "WM" : "JW"
                });
            }
        }





    }
}
