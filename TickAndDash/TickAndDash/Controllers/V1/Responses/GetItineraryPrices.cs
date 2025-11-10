using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDash.Controllers.V1.Responses
{
    public class GetItineraryPrices
    {
        public int Id { get; set; }
        public string ItineraryName { get; set; }

        public double Price { get; set; }
    }


    public class GetItinerariesRequest
    {
        public int Id { get; set; }
        public string ItineraryName { get; set; }

    }
}
