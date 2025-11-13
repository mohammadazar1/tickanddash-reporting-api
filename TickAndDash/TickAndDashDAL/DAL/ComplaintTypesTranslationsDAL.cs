using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class ComplaintTypesTranslationsDAL : BaseDAL, IComplaintTypesTranslationsDAL
    {
        public ComplaintTypesTranslationsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<ComplaintTypeTranslation>> GetAllComplaintsTypeAsync(string lang)
        {
            string query = @"SELECT * FROM Complaints.ComplaintTypesTranslations
                              WHERE Lang = @lang
                            ";


            using (var cn = GetTickAndDashConnection())
            {
                var res = await cn.QueryAsync<ComplaintTypeTranslation>(query, new { lang });
                return res.ToList();
            }
        }

        public async Task<ComplaintTypeTranslation> GetComplaintTypeAsync(int id, string lang)
        {
            string query = @"SELECT * FROM Complaints.ComplaintTypesTranslations
                             WHERE Lang = @lang and ComplaintTypesId = @id";

            using (var cn = GetTickAndDashConnection())
            {
                return await cn.QueryFirstOrDefaultAsync<ComplaintTypeTranslation>(query, new { lang, id });
            }
        }
    }
}
