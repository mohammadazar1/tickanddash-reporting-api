using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IComplaintsDAL
    {
        Task<List<Complaint>> GetComplaintByStatusAsync(ComplaintEnum complaintEnum);
        Task<Complaint> GetComplaintByIdAsync(int id);
        Task<List<Complaint>> GetComplaintsByUserId(int userId);

        Task<bool> InsertComplaintAsync(Complaint complaint);
    }
}
