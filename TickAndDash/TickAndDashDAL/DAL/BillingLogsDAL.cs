using Dapper;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class BillingLogsDAL : BaseDAL, IBillingLogsDAL
    {
        public BillingLogsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> InsertBillingLogAsync(BillingLogs billingLogs)
        {
            string query = @"INSERT INTO BillingLogs (UserId,BillingTimeStamp,Status,Price,Response)
                             VALUES (@UserId, @BillingTimeStamp, @Status, @Price, @Response)";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    billingLogs.UserId,
                    billingLogs.BillingTimeStamp,
                    billingLogs.Status,
                    billingLogs.Price,
                    billingLogs.Response
                }) > 0;
            }

        }
    }
}
