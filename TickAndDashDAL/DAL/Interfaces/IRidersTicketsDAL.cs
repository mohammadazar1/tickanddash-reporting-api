using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IRidersTicketsDAL
    {
        Task<int> AddRidersTicketAsync(RidersTickets ridersTickets);
        Task<string> GetRiderTicketAsync(int riderQId);

    }
}
