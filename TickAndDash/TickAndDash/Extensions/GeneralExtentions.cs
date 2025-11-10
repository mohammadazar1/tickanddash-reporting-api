using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using TickAndDash.Services.ServicesDtos;

namespace TickAndDash.Extensions
{
    public static class GeneralExtentions
    {

        public static bool IsPalMobileNumber(this string msisdn)
        {
            bool success = false;
            Regex msisdnMatch = new Regex("^((\\+972|\\+970|972|00972|00970|972|970)(5)[0123456789]{8}$)|^(0?5)[0123456789]{8}$|^(0?5)[0123456789]{8}$");

            // With Israeli occupation operators 054xx..
            //Regex msisdnMatch = new Regex("^((00972|00970|972|970)(56|59|54)[0123456789]{7}$)|^(0?59)[0123456789]{7}$|^(0?56)[0123456789]{7}$|^(0?54)[0123456789]{7}$");
            Match match = msisdnMatch.Match(msisdn);

            if (match.Success)
            {
                success = true;
                return success;
            }

            return success;
        }


        public static string ConvertMobileNumberFormate(this string msisdn)
        {
            return $"972{msisdn.Substring(msisdn.Length - 9)}";
        }


        public static DeviceInfo GetDeviceInfo(this IActionContextAccessor _actionContextAccessor)
        {
            DeviceInfo deviceInfo = new DeviceInfo();
            var httpContext = _actionContextAccessor.ActionContext.HttpContext; ;
            string userAgent = httpContext.Request.Headers?["User-Agent"].ToString();

            if (userAgent != null)
            {
                string[] info = userAgent.Split('(', ')')[1]?.Split(';'); // [1] means it selects second part of your what you split parts of your string. (Zero based)
                if (info.Count() >= 3)
                {
                    deviceInfo.Operation = info[0];
                    deviceInfo.OSVersion = info[1];
                    deviceInfo.DeviceModel = info[2];
                }
                if (info.Count() == 2)
                {
                    deviceInfo.Operation = info[0];
                    deviceInfo.OSVersion = info[1];
                }

                if (info.Count() == 1)
                {
                    deviceInfo.Operation = info[0];
                }
            }

            deviceInfo.IP = httpContext.Connection.RemoteIpAddress?.ToString();
            deviceInfo.UserAgent = userAgent;

            return deviceInfo;
        }


















    }
}
