using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using TickAndDash.DALs.Models;
using TickAndDash.Services.ServicesDtos;

namespace TickAndDash.DALs.Interfaces
{
    //public class RidersDAL : BaseDAL, IRidersDAL
    //{
    //    private readonly string _tableName = "Riders";
    //    private readonly string _schema = "dbo";

    //    public RidersDAL(IConfiguration configuration) : base(configuration)
    //    {

    //    }

    //    public Riders GetRiderByMobileNumber(Riders rider)
    //    {
    //        string query = @$"SELECT Id,Name,MobileNumber,CreationDate,RoleId, UserId , LoginPincode,GeneratedPincode, IsLoggedIn, UserAgent, DeviceModel, OSVersion, Operation, IP
    //            FROM {_schema}.{_tableName} rid , dbo.Users usr
    //            where  usr.Id = rid.UserId
    //            and usr.MobileNumber = @MobileNumber";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            var riderUser = sqlConnection.Query<Users, Riders, Riders>(query, (usr, rid) =>
    //            {
    //                rid.User = usr;
    //                return rid;
    //            }, new { rider.User.MobileNumber }, splitOn: "UserId").FirstOrDefault();


    //            return riderUser;
    //        }

    //    }

    //    public bool UpdateRiderLoginInfo(Riders riders, DeviceInfo deviceInfo)
    //    {
    //        string query = @$"UPDATE [{_schema}].[{_tableName}]
    //                          SET LoginPincode = @LoginPincode,  UserAgent = @UserAgent, DeviceModel = @DeviceModel, OSVersion = @OSVersion, Operation = @Operation, IP = @IP
    //                          WHERE UserId = @UserId";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            int affectedRows = sqlConnection.Execute(query, new { riders.UserId, riders.LoginPincode, deviceInfo.UserAgent, deviceInfo.DeviceModel, deviceInfo.OSVersion, deviceInfo.Operation, deviceInfo.IP });

    //            return affectedRows > 0;
    //        }
    //    }

    //    public bool UpdateRiderGeneratedPincode(Riders rider)
    //    {
    //        string query = @$"UPDATE [{_schema}].[{_tableName}]
    //                          SET GeneratedPincode = @GeneratedPincode
    //                          WHERE UserId = @UserId";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            int affectedRows = sqlConnection.Execute(query, new { rider.UserId, rider.GeneratedPincode });

    //            return affectedRows > 0;
    //        }
    //    }

    //    public bool UpdateRiderLogginStauts(int userId, bool isLoggedIn)
    //    {
    //        string query = @$"UPDATE [{_schema}].[{_tableName}]
    //                          SET IsLoggedIn = @isLoggedIn
    //                           , IsRegisterd = 1
    //                          WHERE UserId = @userId";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            int affectedRows = sqlConnection.Execute(query, new { userId, isLoggedIn });

    //            return affectedRows > 0;
    //        }
    //    }

    //    public Riders GetRegisteredRiders(string mobileNumber)
    //    {
    //        string query = @$"SELECT Id,Name,MobileNumber,CreationDate, RoleId, UserId ,LoginPincode, ResentPincode, IsLoggedIn
    //            FROM {_schema}.{_tableName} rid , dbo.Users usr
    //            where  usr.Id = rid.UserId
    //            and usr.MobileNumber = @mobileNumber 
    //            and rid.IsRegisterd = 1
    //            ";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            var riders = sqlConnection.Query<Users, Riders, Riders>(query, (usr, rid) =>
    //            {
    //                rid.User = usr;
    //                return rid;
    //            }, new { mobileNumber }, splitOn: "UserId").FirstOrDefault();


    //            return riders;
    //        }
    //    }

    //    public bool UpdateRiderGeneratedPincode(int Pincode)
    //    {
    //        string query = @$"UPDATE [{_schema}].[{_tableName}]
    //                          SET GeneratedPincode = @Pincode
    //                          WHERE UserId = @userId";

    //        using (var sqlConnection = GetTickAndDashConnection())
    //        {
    //            int affectedRows = sqlConnection.Execute(query, new { Pincode });

    //            return affectedRows > 0;
    //        }
    //    }
    //}
}
