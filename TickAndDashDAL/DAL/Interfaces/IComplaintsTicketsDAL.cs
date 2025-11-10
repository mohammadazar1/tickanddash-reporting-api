using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IComplaintsTicketsDAL
    {
        Task<bool> InsertComplaintStoryAsync(ComplaintsTickets complaintStory);
        Task<List<ComplaintsTickets>> GetComplaintStoriesAsync();

        Task<List<Complaint>> GetComplaintTickets(List<Complaint> complaints);

    }
}
