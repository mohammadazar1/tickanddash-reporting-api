using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class ComplaintsSubTypeTranslationsDAL : BaseDAL, IComplaintsSubTypeTranslations
    {
        public ComplaintsSubTypeTranslationsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<ComplaintSubTypeTranslation>> GetAllComplaintsTypeSubTypeAsync(int complaintTypesId, string lang)
        {
            string query = @"SELECT supCompTrans.* 
                             FROM  Complaints.ComplaintsSubTypeTranslations                                       supCompTrans, Complaints.ComplaintsSubType supComp
                             WHERE  supComp.Id= supCompTrans.ComplaintsSubTypeId 
                                   and supCompTrans.Lang = @lang and supComp.ComplaintTypesId =                @complaintTypesId
                            ";
            using (var cn = GetTickAndDashConnection())
            {
                var result = await cn.QueryAsync<ComplaintSubTypeTranslation>(query, new { complaintTypesId, lang });

                return result.ToList();
            }

        }
    }
}
