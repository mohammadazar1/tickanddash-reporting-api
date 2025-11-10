using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IRidersDAL
    {

        Task<Riders> GetRiderByMobileNumberAsync(string MobileNumber);
        Task<Riders> GetRiderByIdAsync(int id);
        Task<string> GetRiderMobileOS(int userId);
        Task<List<Riders>> GetSubscribedRidersToBeRenewedAsync();
        bool AddRiders(Riders riders); 
        Task<bool> UpdateRiderGeneratedPincodeAsync(Riders riders); 
        bool UpdateRiderLoginInfo(Riders riders);
        Task<bool> UpdateRiderAsync(Riders rider);
        Task<bool> UpdateRiderNextBillingDateAsync(int riderId);
    }

    
}
