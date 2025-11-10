using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDash.Services.Interfaces
{
    public interface ISMSService
    {
        //Task<bool> SendSMSToUserAsync(string msisdn, string msg);

        Task<bool> SendSMSToUserAsync(string msisdn, string msg); // used
    }
}
