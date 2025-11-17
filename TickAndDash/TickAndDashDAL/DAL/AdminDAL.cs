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
                // Try with JOIN first (if Users and Roles exist)
                try
                {
                    var sql = $@"SELECT a.*, r.Id RoleId, r.Role Role  FROM Admins a, Users u, Roles r 
                                    WHERE a.Username = @Username AND u.RoleId = r.Id AND a.UserId = u.Id";
                    var admin = connection
                        .Query<Admin>(sql, new { Username = username })
                        .FirstOrDefault();
                    
                    if (admin != null)
                    {
                        return admin;
                    }
                }
                catch
                {
                    // If JOIN fails (e.g., Users or Roles table is empty), try without JOIN
                }
                
                // Fallback: Get admin without JOIN (if Users/Roles are empty)
                var fallbackSql = @"SELECT * FROM Admins WHERE Username = @Username";
                var adminFallback = connection
                    .Query<Admin>(fallbackSql, new { Username = username })
                    .FirstOrDefault();
                
                // Set default role if not set
                if (adminFallback != null && string.IsNullOrWhiteSpace(adminFallback.Role))
                {
                    adminFallback.Role = "Admin";
                }
                
                return adminFallback;
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
