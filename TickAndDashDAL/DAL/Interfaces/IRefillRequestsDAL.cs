using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IRefillRequestsDAL
    {
        Task<bool> IsThereAnyRequestsToTransferInTheLastNminutesAsync(int riderId, int driverId);
        Task<RefillRequest> GetRefillRequest(int riderId, int driverId, decimal amount);
        Task<bool> InsertRefillRequest(RefillRequest refill);

        Task<bool> UpdateRefillRequest(RefillRequest refill);
        Task<int> GetCountOfDriverRefillRequestsAsync(int driverId);
        Task<List<RefillRequest>> GetAllRefillRequestsAsync(int driverId);
    }
}
