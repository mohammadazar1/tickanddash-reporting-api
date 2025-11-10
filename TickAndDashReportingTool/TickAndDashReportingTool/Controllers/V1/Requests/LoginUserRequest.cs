using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class LoginUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
