using System.Collections.Generic;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class RiderQueueService : IRiderQueueService
    {
        private readonly IRidersQueueDAL _riderQueueDAL;

        public RiderQueueService(IRidersQueueDAL riderQueueDAL)
        {
            _riderQueueDAL = riderQueueDAL;
        }

        public List<RidersQueue> GetAll(int Id)
        {
            return _riderQueueDAL.GetAllByPickupStationId(Id);
        }
    }
}
