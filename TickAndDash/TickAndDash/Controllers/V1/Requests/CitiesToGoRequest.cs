using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDash.Controllers.V1.Requests
{
    public class CitiesToGoRequest
    {   /// <summary>
        /// The country
        /// </summary>
        /// <example>31.909190</example>
        public decimal Lat { get; set; }
        /// <summary>
        /// The country
        /// </summary>
        /// <example>35.207891</example>
        public decimal Long { get; set; }
    }
}
