using Dapper;
using System;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class CallbacksDAL : BaseDAL, ICallbacksDAL
    {
        public CallbacksDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> InsertCallbackAsync(Callback callback)
        {
            string query = @" insert into Callbacks(BillingStatus, MSISDN, TimeStamp, CreatedAt, NextActionDate)
                              values(@BillingStatus, @MSISDN, @TimeStamp, @CreatedAt, @NextActionDate)";

            string TimeStamp = callback.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string NextActionDate = callback.NextActionDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    BillingStatus = callback.BillingStatus.ToLower(),
                    callback.MSISDN,
                    TimeStamp,
                    NextActionDate,
                    CreatedAt
                }) > 0;
            }
        }
    }
}
