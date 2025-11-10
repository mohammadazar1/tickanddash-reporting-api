using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using TickAndDash.DALs.Interfaces;
using TickAndDash.DALs.Models;

namespace TickAndDash.DALs
{
    //public class UserDAL : BaseDAL, IUserDAL
    //{

        //private readonly string _tableName = "Users";
        //private readonly string _schema = "dbo";
        //public UserDAL(IConfiguration configuration) : base(configuration)
        //{

        //}



        //public int AddUser(Users user)
        //{
        //    string query = @$"INSERT INTO [{_schema}].[{_tableName}] (Name ,MobileNumber ,CreationDate ,RoleId) 
        //                     VALUES(@Name,@MobileNumber,@CreationDate, @RoleId); 
        //                     SELECT SCOPE_IDENTITY()
        //                    ";
        //    try
        //    {
        //        using (var sqlConnection = GetTickAndDashConnection())
        //        {
        //            int id = sqlConnection.Query<int>(query, new { user.MobileNumber, user.Name, user.CreationDate, user.RoleId, user.IsActive }).FirstOrDefault();

        //            return id;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return 0;
        //}


        //public Users GetUser(int id, bool isActive)
        //{
        //    string query = @"SELECT * 
        //                    FROM  [{_schema}].[{_tableName}]
        //                    where id = @id";
        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Query<Users>(query, new { id }).FirstOrDefault();
        //    }
        //}

        //public Users GetUserByMobileNumber(Users users)
        //{
        //    string query = @$"SELECT * 
        //                    FROM  [{_schema}].[{_tableName}]
        //                    where MobileNumber = @MobileNumber";
        //    try
        //    {
        //        using (var sqlConnection = GetTickAndDashConnection())
        //        {
        //            return sqlConnection.Query<Users>(query, new { users.MobileNumber }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        //public Users GetUser(string mobileNumber, bool isActive)
        //{
        //    string query = @$"SELECT * 
        //                    FROM  [{_schema}].[{_tableName}]
        //                    where MobileNumber = @mobileNumber
        //                    and IsActive = @isActive
        //                    ";
        //    try
        //    {
        //        using (var sqlConnection = GetTickAndDashConnection())
        //        {
        //            return sqlConnection.Query<Users>(query, 
        //                new { mobileNumber, isActive }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        //public bool UpdateUserRefreshToken(Users user)
        //{
        //    string query = @$"  update dbo.Users set RefreshToken = @RefreshToken
        //                        where id = @Id";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        int affectedRows = sqlConnection.Execute(query, new { user.Id, user.RefreshToken });

        //        return affectedRows > 0;
        //    }
        //}

        //public bool UpdateUser(int userId, bool isActive)
        //{
        //    string query = @$"  update dbo.Users set IsActive = @isActive
        //                        where id = @userId";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        int affectedRows = sqlConnection.Execute(query, new { userId, isActive });

        //        return affectedRows > 0;
        //    }
        //}

       
    //}
}
