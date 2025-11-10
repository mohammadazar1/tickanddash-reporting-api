using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class DriversLoginRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "يرجى ادخال رقم الرخصة لاستكمال العملية")]
        public string LicenseNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "يرجى ادخال رمز المرور لاستكمال العملية")]
        public string Password { get; set; }

        /// <summary>
        /// FCMToken
        /// </summary>
        /// <example>cYV_LSoITQqD2QYoLrrb1B:APA91bH13UF8NSZkk0tOy7xBPPuD18n574uTuAJd1TQxs4VloXsRm98o22hQOooMAavHD7zLtWOrY1gGiVcDiUjsHzkBbvlZ2b2XaugQUrlDU0AmHV4Bc2FZ3jxPdrC-KwpJh27tnzeX
        /// </example>
        [Required(AllowEmptyStrings = true)]
        //[MinLength(100)]
        public string FCMToken { get; set; }

        public string MobileOS { get; set; } = "";
    }
}
