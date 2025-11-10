using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashDAL.Models.CustomModels;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IUserTransactionsDAL
    {
        Task<List<UserTransactions>> GetUserTransactionsAsync(int userId, DateTime fromDate, DateTime toDate, string languge);
        //Task<UserTransactions> GetUserTransactions(int posId, string mobileNumber, DateTime fromDate, DateTime toDate);
        Task<bool> AddUserTransactionAsync(UserTransactions userTransactions);
        Task<List<POSTransactions>> GetPOSTransactionsAsync(int posId, string mobileNumber, DateTime fromDate, DateTime toDate, bool isPos);

    }
}
