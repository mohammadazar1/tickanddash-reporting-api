using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ITicketService
    {
        Task<string> GenerateManualTicket(Controllers.V1.ManualTicketRequest manualTicketRequest);
    }
}
