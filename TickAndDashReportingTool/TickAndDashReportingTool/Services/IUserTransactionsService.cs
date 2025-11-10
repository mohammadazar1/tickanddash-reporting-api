using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashDAL.Models.CustomModels;

namespace TickAndDashReportingTool.Services
{
    public interface IUserTransactionsService
    {
        public Task<List<POSTransactions>> GetPOSTransactions(int posId, string mobileNumber, DateTime fromDate, DateTime toDate, bool isPos);
        Task<bool> InsertUserTransactions(UserTransactions userTransaction);

    }
}
