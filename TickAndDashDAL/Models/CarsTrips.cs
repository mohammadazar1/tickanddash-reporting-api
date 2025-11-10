using System;
using TickAndDashDAL.DAL;

namespace TickAndDashDAL.Models
{
    public class CarsTrips
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int CarsQueueId { get; set; }
        public CarsQueue CarsQueue { get; set; }
        public int DriverId { get; set; }
        public int CarId { get; set; }

        public Driver Driver { get; set; }
        public User User { get; set; }

    }
}
