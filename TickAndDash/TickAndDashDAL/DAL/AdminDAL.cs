using Dapper;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class AdminDAL : BaseDAL, IAdminDAL
    {
        public AdminDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public Admin GetByUserId(int userId)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = "SELECT * FROM Admins WHERE UserId = @UserId";
                return connection
                    .Query<Admin>(sql, new { UserId = userId })
                    .FirstOrDefault();
            }
        }

        public Admin GetByUserName(string username)
        {
            using (var connection = GetTickAndDashConnection())
            {
                // First, try to get admin directly (simplest query)
                var directSql = @"SELECT * FROM Admins WHERE Username = @Username";
                var admin = connection
                    .Query<Admin>(directSql, new { Username = username })
                    .FirstOrDefault();
                
                if (admin == null)
                {
                    return null; // Admin not found
                }
                
                // Try to get role from Users and Roles tables if they exist
                try
                {
                    var roleSql = @"SELECT r.Role FROM Admins a
                                   LEFT JOIN Users u ON a.UserId = u.Id
                                   LEFT JOIN Roles r ON u.RoleId = r.Id
                                   WHERE a.Username = @Username";
                    var role = connection
                        .Query<string>(roleSql, new { Username = username })
                        .FirstOrDefault();
                    
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        admin.Role = role;
                    }
                }
                catch
                {
                    // If JOIN fails, continue with default role
                }
                
                // Set default role if not set
                if (string.IsNullOrWhiteSpace(admin.Role))
                {
                    admin.Role = "Admin";
                }
                
                return admin;
            }
        }

        public bool Insert(Admin admin)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = @"BEGIN TRANSACTION
	                            INSERT INTO Users (Name, RoleId)
		                            VALUES('', 1)
	
	                            DECLARE @UserId INT = (SELECT SCOPE_IDENTITY())

	                            INSERT INTO Admins (UserId, MSISDN, Username, Password)
                                    OUTPUT Inserted.UserId
		                            VALUES(@UserId, @MSISDN, @Username, @Password)
                           COMMIT TRANSACTION";

                var reslut = connection.Execute(sql,
                    new
                    {
                        MSISDN = admin.MSISDN,
                        Username = admin.Username,
                        Password = admin.Password
                    });

                return reslut > 0;
            }
        }
    }
}
