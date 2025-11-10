using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IRiderQueueService
    {
        List<RidersQueue> GetAll(int Id);
    }
}
