using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IRidersTicketsServices
    {
        Task<int> AddRidersTickAsync(RidersTickets ridersTickets);
        Task<string> GetRiderTicketAsync(int riderQId);
    }
}
