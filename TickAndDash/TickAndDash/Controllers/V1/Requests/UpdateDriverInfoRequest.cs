using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class UpdateDriverInfoRequest 
    {
        /// <summary>
        /// Name
        /// </summary>
        /// <example>mohammad</example>
        public string Name { get; set; } = "";

        /// <summary>
        /// password
        /// </summary>
        /// <example>123456</example>
        public string password { get; set; } = "";
        public string oldPassword { get; set; } = "";
    }


    public class UpdateUserLanguage
    {  /// <summary>
       /// Language
       /// </summary>
       /// <example>Ar</example>
       [Required(AllowEmptyStrings = false)]
        public string Language { get; set; }
    }
}
