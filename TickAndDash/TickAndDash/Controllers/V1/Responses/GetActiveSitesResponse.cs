using System.Collections.Generic;

namespace TickAndDash.Controllers.V1.Responses
{


    public class GetActiveSitesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GetDriverActiveReservationResponse
    {

        public PickupStationsResponse From { get; set; }
        public List<PickupStationsResponse> To { get; set; }
        public int Turn { get; set; }

    }

}
