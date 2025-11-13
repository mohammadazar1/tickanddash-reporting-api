using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class ComplaintsDAL : BaseDAL, IComplaintsDAL
    {
        public ComplaintsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Complaint> GetComplaintByIdAsync(int id)
        {
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.GetAsync<Complaint>(id);
            }
        }

        public async Task<List<Complaint>> GetComplaintByStatusAsync(ComplaintEnum complaintEnum)
        {
            string query = $@" SELECT c.*, u.Id, u.Name
                              From   Complaints c, Users u
                              WHERE  c.UserId = u.Id
                                     and c.Status = {complaintEnum}";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<Complaint>(query);
                return result.ToList();
            }
        }

        public async Task<List<Complaint>> GetComplaintsByUserId(int userId)
        {
            string query = @"SELECT * From Complaints
                             WHERE  UserId = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<Complaint>(query, new { userId });

                return result.ToList();
            }
        }

        public async Task<bool> InsertComplaintAsync(Complaint complaint)
        {
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.InsertAsync(complaint) > 0;
            }
        }




    }
}
