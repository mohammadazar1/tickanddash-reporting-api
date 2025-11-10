using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IComplaintsSubTypeTranslations
    {
        Task<List<ComplaintSubTypeTranslation>> GetAllComplaintsTypeSubTypeAsync(int complaintTypesId, string lang);

    }
}
