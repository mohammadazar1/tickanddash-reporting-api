using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDash.Controllers.V1.Responses
{
    public class GetDriverConfigResponse
    {
        /// <summary>
        /// SeatsCount
        /// </summary>
        /// <example>7</example>
        public int SeatsCount { get; set; }
        public string Language { get; set; }
    }
}
