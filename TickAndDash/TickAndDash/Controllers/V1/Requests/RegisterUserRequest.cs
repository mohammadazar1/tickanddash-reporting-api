using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{

    public class RegisterUserRequest
    {

        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }
        //public DateTime CreationDate { get; set; }
        //public int RoleId { get; set; }
        //public bool IsActive { get; set; }
    }


    public class UpdateRiderInfo
    {  /// <summary>
       /// Name
       /// </summary>
       /// <example>mohammad</example>
        public string Name { get; set; } = " ";

        /// <summary>
        /// Gender
        /// </summary>
        /// <example>M</example>
        public char Gender { get; set; } = ' ';
    }
}
