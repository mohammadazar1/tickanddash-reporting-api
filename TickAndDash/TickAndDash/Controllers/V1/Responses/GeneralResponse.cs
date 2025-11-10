using System;

namespace TickAndDash.Controllers.V1.Responses
{
    public class GeneralResponse<T> where T : new()
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public T Data { get; set; } = new T();
        //public Guid? ErrorId { get; set; }
    }

    public class GeneralResponse2<T> where T : new()
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public T data { get; set; } = new T();
        //public Guid? ErrorId { get; set; }
    }


    public class RidersBookingResponse
    {
        public int CountOfCars { get; set; }
        public string status { get; set; }
    }

    public class ActiveReservationResponse
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public int Turn { get; set; }
        public string Status { get; set; }
        public bool IsSkiped { get; set; }
        public PickupStationsResponse PickupStation { get; set; } = new PickupStationsResponse();
        public int CountOfSeats { get; set; }
        public string Ticket { get; set; }

        public string CarCode { get; set; }
    }
}
