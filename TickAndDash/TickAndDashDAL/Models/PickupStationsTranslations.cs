using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class PickupStationsTranslations
    {
        public int Id { get; set; }
        public int PickupStationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }

    }
}
