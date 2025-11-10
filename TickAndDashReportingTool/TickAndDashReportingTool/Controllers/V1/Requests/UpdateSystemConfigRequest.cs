using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class UpdateSystemConfigRequest
    {
        public int Id { get; set; }
        public string SettingValue { get; set; }
    }
}
