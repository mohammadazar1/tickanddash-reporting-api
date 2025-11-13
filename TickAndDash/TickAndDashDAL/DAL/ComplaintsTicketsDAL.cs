using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class ComplaintsTicketsDAL : BaseDAL, IComplaintsTicketsDAL
    {
        public ComplaintsTicketsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<ComplaintsTickets>> GetComplaintStoriesAsync()
        {
            string query = $@"";


            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<ComplaintsTickets>(query);

                return result.ToList();
            }
        }

        public async Task<List<Complaint>> GetComplaintTickets(List<Complaint> complaints)
        {
            string query = @"select ct.* From Complaints c, ComplaintsTickets ct
                                        where c.Id = ct.ComplaintId
                                        and c.Id = @Id
                                        order by ct.CreationDate asc";

            throw new Exception();
            //using (var sqlConnection = GetTickAndDashConnection())
            //{
            //    foreach (var complaint in complaints)
            //    {
            //        var result = await sqlConnection.QueryAsync<ComplaintsTickets>(query, new { complaint.Id });
            //        var tickets = result.ToList();
            //        if (tickets != null)
            //        {
            //            complaint.complaintsTickets = tickets;
            //        }
            //    }
            //}
            //return complaints;
        }

        public async Task<bool> InsertComplaintStoryAsync(ComplaintsTickets complaintStory)
        {
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.InsertAsync(complaintStory) > 0;
            }
        }
    }
}
