using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class ComplaintsServices : IComplaintsServices
    {
        private readonly IComplaintsDAL _complaintsDAL;
        private readonly IComplaintsTicketsDAL _complaintsTicketsDAL;
        private readonly IComplaintTypesTranslationsDAL _complaintTypesTranslationsDAL;
        private readonly IComplaintsSubTypeTranslations _complaintsSubTypeTranslations;

        public ComplaintsServices(IComplaintsDAL complaintsDAL, IComplaintsTicketsDAL complaintsTicketsDAL, IComplaintTypesTranslationsDAL complaintTypesTranslationsDAL, IComplaintsSubTypeTranslations complaintsSubTypeTranslations)
        {
            _complaintsDAL = complaintsDAL;
            _complaintsTicketsDAL = complaintsTicketsDAL;
            _complaintTypesTranslationsDAL = complaintTypesTranslationsDAL;
            _complaintsSubTypeTranslations = complaintsSubTypeTranslations;
        }

        public async Task<Complaint> GetComplaintByIdAsync(int id)
        {
            return await _complaintsDAL.GetComplaintByIdAsync(id);
        }

        public async Task<bool> InsertComplaintAsync(Complaint complaint)
        {
            return await _complaintsDAL.InsertComplaintAsync(complaint);
        }

        public async Task<bool> AddComplaintReplyAsync(ComplaintsTickets complaintStory)
        {
            return await _complaintsTicketsDAL.InsertComplaintStoryAsync(complaintStory);
        }

        public async Task<List<Complaint>> GetComplaintsByUserId(int userId)
        {
            return await _complaintsDAL.GetComplaintsByUserId(userId);
        }

        public async Task<List<Complaint>> GetComplaintTickets(List<Complaint> complaints)
        {
            return await _complaintsTicketsDAL.GetComplaintTickets(complaints);
        }

        public async Task<List<ComplaintTypeTranslation>> GetAllComplaintsTypeAsync(string lang)
        {
            return await _complaintTypesTranslationsDAL.GetAllComplaintsTypeAsync(lang);
        }

        public async Task<List<ComplaintSubTypeTranslation>> GetAllComplaintsTypeSubTypeAsync(int complaintTypesId, string lang)
        {
            return await _complaintsSubTypeTranslations.GetAllComplaintsTypeSubTypeAsync(complaintTypesId, lang);
        }
    }
}
