using Dapper.Contrib.Extensions;
using System;

namespace TickAndDashDAL.Models
{
    [Table("RefillRequests")]
    public class RefillRequest
    {
        public int Id { get; set; }

        [Computed]
        public Riders Rider { get; set; }
        public int RiderId { get; set; }

        public int DriverId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsHandled { get; set; }

        public bool IsSuccess { get; set; }

    }

}
