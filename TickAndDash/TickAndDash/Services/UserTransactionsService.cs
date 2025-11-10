using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class UserTransactionsService : IUserTransactionsService
    {

        private IUserTransactionsDAL _userTransactionsDAL;
        private readonly IActionContextAccessor _actionContextAccessor;
        public UserTransactionsService(IUserTransactionsDAL userTransactionsDAL, IActionContextAccessor actionContextAccessor)
        {
            _userTransactionsDAL = userTransactionsDAL;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<bool> AddUserTransactionAsync(UserTransactions userTransactions)
        {
            return await _userTransactionsDAL.AddUserTransactionAsync(userTransactions);
        }

        public async Task<List<UserTransactions>> GetUserTransactionsAsync(int userId, DateTime fromDate, DateTime toDate)
        {
            string language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers?["Content-Language"].ToString() ?? "ar";

            return await _userTransactionsDAL.GetUserTransactionsAsync(userId, fromDate, toDate, language);
        }
    }
}
