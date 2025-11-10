using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Exceptions
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpStatusException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

    }
}
