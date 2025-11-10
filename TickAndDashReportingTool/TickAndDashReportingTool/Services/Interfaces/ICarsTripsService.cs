using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ICarsTripsService
    {
        IList<CarsTrips> GetAll();

        Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId);

        Task<int> GetCarTripByCarsQueueIDAsync(int carqId);

    }
}
