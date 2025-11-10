using Dapper;
using System.Linq;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class AdminDAL : BaseDAL, IAdminDAL
    {
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
                var sql = $@"SELECT a.*, r.Id RoleId, r.Role Role  FROM Admins a, Users u, Roles r 
                                WHERE a.Username = @Username AND u.RoleId = r.Id AND a.UserId = u.Id";
                return connection
                    .Query<Admin>(sql, new { Username = username })
                    .FirstOrDefault();
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
