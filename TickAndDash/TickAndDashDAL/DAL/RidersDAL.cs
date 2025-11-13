using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class RidersDAL : BaseDAL, IRidersDAL
    {
        public RidersDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private readonly string _RidersTable = "Riders";
        private readonly string _defaultschema = "dbo";

        public RidersDAL() : base()
        {
        }

        public async Task<Riders> GetRiderByMobileNumberAsync(string mobileNumber)
        {
            string query = $@"SELECT usr.Id, usr.Name, usr.CreationDate, usr.RoleId, usr.FCMToken, usr.token, 
                                     rid.UserId ,rid.MobileNumber, rid.LoginPincode, 
                                     rid.GeneratedPincode, rid.Gender, rid.token,
                                     rid.SubscriptionDate, rid.IsSubscribed, rid.ActiveSubscriptionPeriod
                              FROM {_defaultschema}.{_RidersTable} rid , dbo.Users usr
                              WHERE  usr.Id = rid.UserId
                              AND rid.MobileNumber = @MobileNumber";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var riderUser = await sqlConnection.QueryAsync<User, Riders, Riders>(query, (usr, rid) =>
                 {
                     rid.User = usr;
                     return rid;
                 }, new { mobileNumber },
                 splitOn: "UserId");

                return riderUser.FirstOrDefault();
            }
        }

        public async Task<Riders> GetRiderByIdAsync(int id)
        {
            string query = $@"SELECT usr.Id, usr.Name, usr.CreationDate, usr.RoleId, usr.FCMToken, usr.token, usr.Language,
                                     rid.UserId ,rid.MobileNumber, rid.LoginPincode, 
                                     rid.GeneratedPincode, rid.Gender, rid.token
                                     ,rid.SubscriptionDate, rid.ActiveSubscriptionPeriod
                                     ,rid.IsSubscribed
                              FROM {_defaultschema}.{_RidersTable} rid , dbo.Users usr
                              WHERE  usr.Id = rid.UserId
                              AND rid.UserId = @id";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var riderUser = await sqlConnection.QueryAsync<User, Riders, Riders>(query, (usr, rid) =>
                {
                    rid.User = usr;
                    return rid;
                }, new { id },
                 splitOn: "UserId");

                return riderUser.FirstOrDefault();
            }
        }

        public bool UpdateRiderLoginInfo(Riders rider)
        {
            string query = $@"UPDATE [{_defaultschema}].[{_RidersTable}]
                              SET LoginPincode = @LoginPincode
                              WHERE UserId = @UserId";
            //UserAgent = @UserAgent, DeviceModel = @DeviceModel, OSVersion = @OSVersion, Operation = @Operation, IP = @IP

            using (var sqlConnection = GetTickAndDashConnection())
            {
                int affectedRows = sqlConnection.Execute(query, new { rider.UserId, rider.LoginPincode });
                //, rider.UserAgent, rider.DeviceModel, rider.OSVersion, rider.Operation, rider.IP
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateRiderGeneratedPincodeAsync(Riders rider)
        {
            string query = $@"UPDATE [{_defaultschema}].[{_RidersTable}]
                              SET GeneratedPincode = @GeneratedPincode
                              WHERE UserId = @UserId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { rider.UserId, rider.GeneratedPincode }) > 0;
            }
        }

        //public bool UpdateRiderLogginStauts(int userId, bool isLoggedIn)
        //{
        //    string query = $@"UPDATE [{_RidersTable}].[{_RidersTable}]
        //                      SET IsLoggedIn = @isLoggedIn
        //                       , IsRegisterd = 1
        //                      WHERE UserId = @userId";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        int affectedRows = sqlConnection.Execute(query, new { userId, isLoggedIn });

        //        return affectedRows > 0;
        //    }
        //}

        //public Riders GetRegisteredRiders(string mobileNumber)
        //{
        //    string query = $@"SELECT Id,Name,MobileNumber,CreationDate, RoleId, UserId ,LoginPincode, ResentPincode, IsLoggedIn
        //        FROM {_RidersTable}.{_RidersTable} rid , dbo.Users usr
        //        where  usr.Id = rid.UserId
        //        and usr.MobileNumber = @mobileNumber 
        //        and rid.IsRegisterd = 1
        //        ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        var riders = sqlConnection.Query<Users, Riders, Riders>(query, (usr, rid) =>
        //        {
        //            rid.User = usr;
        //            return rid;
        //        }, new { mobileNumber }, splitOn: "UserId").FirstOrDefault();


        //        return riders;
        //    }
        //}

        //public bool UpdateRiderGeneratedPincode(int Pincode)
        //{
        //    string query = $@"UPDATE [{_defaultschema}].[{_RidersTable}]
        //                      SET GeneratedPincode = @Pincode
        //                      WHERE UserId = @userId";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        int affectedRows = sqlConnection.Execute(query, new { Pincode });

        //        return affectedRows > 0;
        //    }
        //}
        public bool AddRiders(Riders riders)
        {
            bool success = false;

            string query = $@"INSERT INTO [{_defaultschema}].[{_RidersTable}] (UserId , MobileNumber ,GeneratedPincode, Token) 
                              VALUES(@UserId,@MobileNumber,@GeneratedPincode,@Token); 
                              SELECT SCOPE_IDENTITY()
                        ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                int affectedRows = sqlConnection.Execute(query, new { riders.UserId, riders.MobileNumber, riders.GeneratedPincode, riders.Token });

                if (affectedRows > 0)
                {
                    success = true;
                }
            }


            return success;
        }

        public async Task<bool> UpdateRiderAsync(Riders rider)
        {
            string query = $@"Update Riders set LoginPincode = ISNULL(NULLIF(@LoginPincode,''), LoginPincode) , GeneratedPincode = ISNULL(NULLIF                  (@GeneratedPincode,''),GeneratedPincode),Gender = ISNULL(NULLIF(@Gender,''), Gender), MobileOs =  ISNULL(NULLIF                     (@MobileOs,''), MobileOs)
                              Where UserId = @UserId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.
                    ExecuteAsync(query, new
                    {
                        rider.LoginPincode,
                        rider.GeneratedPincode,
                        rider.Gender,
                        rider.UserId,
                        rider.MobileOS,
                    }) > 0;
            }
        }

        public async Task<List<Riders>> GetSubscribedRidersToBeRenewedAsync()
        {
            string query = $@"
                              SELECT * 
                              FROM RIDERS
                              Where NextBillingDate <= {DateTime.Now}
                            ";
            using (var sqlConnectino = GetTickAndDashConnection())
            {
                var result = await sqlConnectino.QueryAsync<Riders>
                              (query, new { });

                return result.ToList();

            }
        }

        public async Task<bool> UpdateRiderNextBillingDateAsync(int riderId)
        {
            string query = @"Update Riders SET NextBillingDate = dateadd(dd,30,NextBillingDate)
                             WHERE UserId = @riderId
                            ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { riderId }) > 0;
            }
        }

        public async Task<string> GetRiderMobileOS(int userId)
        {
            string query = @"SELECT MobileOS FROM Riders
                             where UserId = @userId";

            using (var sqlConnectino = GetTickAndDashConnection())
            {
                return (await sqlConnectino.QueryAsync<string>
                                 (query, new { userId })).FirstOrDefault();
            }
        }
    }
}
