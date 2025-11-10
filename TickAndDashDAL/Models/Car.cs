using System;
using System.ComponentModel.DataAnnotations;
using TickAndDashDAL.DAL;

namespace TickAndDashDAL.Models
{
    public class Car
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public int ItineraryId { get; set; }

        public Transportation_Itineraries Transportation_Itineraries { get; set; }

        [MaxLength(30)]
        public string RegistrationPlate { get; set; }
        [MaxLength(30)]
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public int SeatCount { get; set; }
        public int LoggedInDriverId { get; set; }
        public Driver LoggedInDriver { get; set; }
        public User User { get; set; } = new User();    
        public string CarCode { get; set; }
    }
}
