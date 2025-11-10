using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashDAL.Models.CustomModels;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public class UserTransactionsService : IUserTransactionsService
    {

        private readonly IUserTransactionsDAL _userTransactionsDAL;

        public UserTransactionsService(IUserTransactionsDAL userTransactionsDAL)
        {
            _userTransactionsDAL = userTransactionsDAL;
        }

        public async Task<List<POSTransactions>> GetPOSTransactions(int posId, string mobileNumber, DateTime fromDate, DateTime toDate, bool isPos)
        {

            mobileNumber = !string.IsNullOrWhiteSpace(mobileNumber)? $"972{mobileNumber.Substring(mobileNumber.Length - 9)}" : mobileNumber;

            return await _userTransactionsDAL.
                GetPOSTransactionsAsync(posId, mobileNumber, fromDate, toDate, isPos);
        }


        public async Task<bool> InsertUserTransactions(UserTransactions userTransaction)
        {
            return await _userTransactionsDAL.AddUserTransactionAsync(userTransaction);
        }
    }
}
