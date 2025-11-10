using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDashDAL.Enums;

namespace TickAndDash
{
    public static class Helpers
    {
        public static void LogReponse(object response, string paramter, string info)
        {
            Log.
                ForContext(paramter, response, true).
                Information(info);
        }

        public static int GenerateRandomPinCode()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random(Guid.NewGuid().GetHashCode());
            return _rdm.Next(_min, _max);
        }

        //public static DeviceInfo GetDeviceInfo(string userAgent, string ipAddress)
        //{
        //    DeviceInfo deviceInfo = new DeviceInfo();

        //    if (userAgent != null)
        //    {
        //        string[] info = userAgent.Split('(', ')')[1]?.Split(';'); // [1] means it selects second part of your what you split parts of your string. (Zero based)
        //        if (info.Count() >= 3)
        //        {

        //            deviceInfo.Operation = info[0];
        //            deviceInfo.OSVersion = info[1];
        //            deviceInfo.DeviceModel = info[2];
        //        }
        //        if (info.Count() == 2)
        //        {
        //            deviceInfo.Operation = info[0];
        //            deviceInfo.OSVersion = info[1];
        //        }

        //        if (info.Count() == 1)
        //        {
        //            deviceInfo.Operation = info[0];
        //        }
        //    }

        //    deviceInfo.IP = ipAddress;
        //    deviceInfo.UserAgent = userAgent;


        //    return deviceInfo;

        //}

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ArabicToHex(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                try
                {
                    sb.Append(string.Format("{0:X4}", Convert.ToInt32(c)));
                }
                catch //(Exception e)
                {
                    //cannot do this!
                }
            }
            return sb.ToString();
        }

        public static bool ValidateLatAndLong(decimal lat, decimal lng)
        {
            bool success = true;

            if (lat < -90 || lat > 90)
            {
                success = false;
            }

            if (lng < -180 || lng > 180)
            {
                success = false;
            }

            return success;
        }

        public static (int, GeneralResponse<object>) ValidateModelState(ModelStateDictionary modelState, out string message)
        {
            int statusCode = 400;
            message = "عذرًا، خطأ في البيانات المرسلة";
            GeneralResponse<object> validationResponse = new GeneralResponse<object>();

            var errors = new List<string>();

            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }


            var mErrors = modelState.Select(x => x.Value.Errors)
                        .Where(y => y.Count > 0)
                        .ToList();

            string emptyBodyerror = mErrors?.FirstOrDefault()?.FirstOrDefault()?.ErrorMessage ?? "عذرًا، خطأ في البيانات المرسلة";

            if (emptyBodyerror == "A non-empty request body is required.")
            {
                statusCode = 404;
            }

            validationResponse.Message = message;
            validationResponse.Code = ValidationCodes.Param_1.ToString();
            validationResponse.Data = errors;

            return (statusCode, validationResponse);
        }

        public static bool IsValidMobileOS(string os)
        {
            try
            {
                MobileOSEnum mobileOS = (MobileOSEnum)Enum.Parse(typeof(MobileOSEnum), os, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsMobileDevice(HttpRequest r)
        {
            string userAgetnt = r.Headers["User-Agent"];
            string deviceName = "Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini";
            return Regex.IsMatch(userAgetnt, deviceName);
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
