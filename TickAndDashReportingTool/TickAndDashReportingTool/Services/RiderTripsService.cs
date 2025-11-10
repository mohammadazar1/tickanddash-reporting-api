using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class RiderTripsService : IRiderTripsService
    {
        private readonly ITripsRidersDAL _tripsRidersDAL;
        private readonly ITransItinerariesDAL _transItinerariesDAL;

        public RiderTripsService(ITripsRidersDAL tripsRidersDAL, ITransItinerariesDAL transItinerariesDAL)
        {
            _tripsRidersDAL = tripsRidersDAL;
            _transItinerariesDAL = transItinerariesDAL;
        }

        public IList<TripsRiders> GetAll()
        {
            var trips =  _tripsRidersDAL.GetAll();

            foreach (var trip in trips)
            {
                trip.RidersQueue.PickupStations.Transportation_Itineraries = _transItinerariesDAL.GetById(trip.RidersQueue.PickupStations.TransItineraryId);
            }
            return trips;
        }
    }
}
