using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IComplaintsServices
    {
        Task<bool> InsertComplaintAsync(Complaint complaint);
        Task<bool> AddComplaintReplyAsync(ComplaintsTickets complaintStory);
        Task<Complaint> GetComplaintByIdAsync(int id);
        Task<List<Complaint>> GetComplaintsByUserId(int userId);

        Task<List<Complaint>> GetComplaintTickets(List<Complaint> complaints);
        Task<List<ComplaintTypeTranslation>> GetAllComplaintsTypeAsync(string lang);
        Task<List<ComplaintSubTypeTranslation>> GetAllComplaintsTypeSubTypeAsync(int complaintTypesId, string lang);
    }
}
