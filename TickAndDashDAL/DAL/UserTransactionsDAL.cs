using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashDAL.Models.CustomModels;

namespace TickAndDashDAL.DAL
{
    public class UserTransactionsDAL : BaseDAL, IUserTransactionsDAL
    {
        public async Task<bool> AddUserTransactionAsync(UserTransactions userTransactions)
        {
            string query = @"insert into UserTransactions(FromUserId, ToUserId, Amount, CreationDate, Type, UserTransactionTypeId)
                             values(@FromUserId, @ToUserId, @Amount, @CreationDate, @Type, @UserTransactionTypeId)";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    userTransactions.Amount,
                    userTransactions.FromUserId,
                    userTransactions.ToUserId,
                    CreationDate = userTransactions.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    userTransactions.Type,
                    userTransactions.UserTransactionTypeId
                }) > 0;
            }

        }


        public async Task<List<UserTransactions>> GetUserTransactionsAsync(int userId, DateTime fromDate, DateTime toDate, string language)
        {
            //toDate = DateTime.Now;
            //string query = $@"SELECT *
            //                  FROM UserTransactions
            //                  WHERE FromUserId = @userId
            //                        or ToUserId = @userId
            //                        and CreationDate > @fromDate and CreationDate < @toDate
            //               ";
            string query = $@"SELECT Id, FromUserId, ToUserId, Amount, CreationDate, UserTransactionTypeId, UST.Type
                                    FROM UserTransactions US, 
                                    UserTransactionsTypeTranslation UST
                                    WHERE US.UserTransactionTypeId = UST.UserTransactionsTypeId
                                    AND UST.Language = @language
                                    AND (US.FromUserId = @userId
                                    OR  US.ToUserId = @userId)
                                    AND US.CreationDate > @fromDate and US.CreationDate < @toDate
                                    order by US.CreationDate desc
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<UserTransactions>(query, new
                {
                    userId,
                    fromDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    toDate = toDate.ToString("yyyy-MM-dd 23:59:59"),
                    language
                });

                return result.ToList();
            }
        }

        public async Task<List<POSTransactions>> GetPOSTransactionsAsync(int posId, string mobileNumber, DateTime fromDate, DateTime toDate, bool isPos)
        {
            var posQuery = "";
            if (isPos) posQuery = $@" AND u.FromUserId = @posId ";
            else if (posId > 0) posQuery = $@" AND pos.Id = @posId ";
           
            // soso 
            string query = $@"SELECT pos.id as POSId, pos.NameAr as POSName, r.MobileNumber as 'MSISDN', u.Amount, u.Id, u.CreationDate as 'TransferDate'
                                FROM UserTransactions u, Riders r, PointOfSales pos
                                where u.FromUserId = pos.UserId and u.ToUserId = r.UserId
                                      and CreationDate >= @fromDate and CreationDate <= @toDate
                                      {posQuery}
                                      {(!string.IsNullOrWhiteSpace(mobileNumber) ? " and r.MobileNumber = @mobileNumber" : "")}";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<POSTransactions>(query, new
                {
                    mobileNumber,
                    posId,
                    fromDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    toDate = toDate.ToString("yyyy-MM-dd HH:mm:ss"),
                })).ToList();
            }

        }
    }
}
