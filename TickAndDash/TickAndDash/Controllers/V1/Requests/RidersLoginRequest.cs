using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class RidersLoginRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "عذرًا، يرجى ادخال رقم الموبايل")]
        public string MobileNumber { get; set; }

        [Range(1000, 9999, ErrorMessage = "عذرًا رمز التفعيل مكون من 4 خانات فقط")]
        public int Pincode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MobileOperatingSystem { get; set; }

        [Required(AllowEmptyStrings = true)]
        //[MinLength(100)]
        public string FCMToken { get; set; }
        

   

    }
}
