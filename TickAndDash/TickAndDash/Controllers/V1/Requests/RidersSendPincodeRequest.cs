using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class RidersSendPincodeRequest
    {
        /// <summary>
        /// MobileNumber
        /// </summary>
        /// <example>0598660503</example>
        //[SwaggerSchema("The product identifier", ReadOnly = true)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "عذرًا، يرجى ادخال رقم الموبايل")]
        public string MobileNumber { get; set; }


    }
}
