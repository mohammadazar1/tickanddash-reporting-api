using Dapper;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class RidersTicketsDAL : BaseDAL, IRidersTicketsDAL
    {
        public RidersTicketsDAL(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<int> AddRidersTicketAsync(RidersTickets ridersTickets)
        {
            string query = @"
                            INSERT INTO RidersTickets
                                       ( RiderQId, Ticket )
                                 VALUES (@RiderQId ,@Ticket)
                              SELECT SCOPE_IDENTITY()  
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<int>(query, new { ridersTickets.RiderQId, ridersTickets.Ticket })).FirstOrDefault();
            }
        }

        public async Task<string> GetRiderTicketAsync(int riderQId)
        {
            string query = @"SELECT Ticket From RidersTickets
                              WHERE RiderQId = @riderQId
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<string>(query, new { riderQId });

                return result.FirstOrDefault();
            }
        }
    }
}
