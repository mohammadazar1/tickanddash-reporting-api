using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IComplaintTypesTranslationsDAL
    {
        Task<List<ComplaintTypeTranslation>> GetAllComplaintsTypeAsync(string lang);
        Task<ComplaintTypeTranslation> GetComplaintTypeAsync(int id, string lang);
    }
}
