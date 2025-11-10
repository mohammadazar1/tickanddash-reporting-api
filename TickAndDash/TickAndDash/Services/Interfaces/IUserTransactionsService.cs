using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IUserTransactionsService
    {
        Task<List<UserTransactions>> GetUserTransactionsAsync(int userId, DateTime FromDate, DateTime toDate);
        Task<bool> AddUserTransactionAsync(UserTransactions userTransactions);

    }
}
