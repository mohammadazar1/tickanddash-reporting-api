using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ICarsQueuesService
    {
        IList<CarsQueue> GetAll(GetCarsQueueRequest getCarsQueueRequest);
        bool Update(UpdateCarsQueueRequest updateCarsQueueRequest);
        Task<bool> ForceGo(ForceGoRequest forceGoRequest);
        Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId);
    }
}
