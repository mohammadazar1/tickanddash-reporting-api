using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class RidersTicketsServices : IRidersTicketsServices
    {
        private readonly IRidersTicketsDAL _ridersTicketsServices;

        public RidersTicketsServices(IRidersTicketsDAL iRidersTicketsServices)
        {
            _ridersTicketsServices = iRidersTicketsServices;
        }

        public async Task<int> AddRidersTickAsync(RidersTickets ridersTickets)
        {
            return await _ridersTicketsServices.AddRidersTicketAsync(ridersTickets);
        }

        public async Task<string> GetRiderTicketAsync(int riderQId)
        {
            return await _ridersTicketsServices.GetRiderTicketAsync(riderQId);
        }
    }
}
