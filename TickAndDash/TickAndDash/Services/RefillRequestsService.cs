using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class RefillRequestsService : IRefillRequestsService
    {
        private readonly IRefillRequestsDAL _refillRequestsDAL;

        public RefillRequestsService(IRefillRequestsDAL refillRequestsDAL)
        {
            _refillRequestsDAL = refillRequestsDAL;
        }

        public async Task<List<RefillRequest>> GetAllRefillRequestsAsync(int driverId)
        {
            return await _refillRequestsDAL.GetAllRefillRequestsAsync(driverId);
        }

        public async Task<int> GetCountOfDriverRefillRequestsAsync(int driverId)
        {
            return await _refillRequestsDAL.GetCountOfDriverRefillRequestsAsync(driverId);

        }

        public async Task<RefillRequest> GetRefillRequest(int riderId, int driverId, decimal amount)
        {
            return await _refillRequestsDAL.GetRefillRequest(riderId, driverId,  amount);
        }

        public async Task<bool> InsertRefillRequest(RefillRequest refill)
        {
            return await _refillRequestsDAL.InsertRefillRequest(refill);
        }

        public async Task<bool> IsThereAnyRequestsToTransferInTheLastNminutesAsync(int riderId, int driverId)
        {
            return await _refillRequestsDAL.IsThereAnyRequestsToTransferInTheLastNminutesAsync(riderId, driverId);
        }

        public async Task<bool> UpdateRefillRequest(RefillRequest refill)
        {
            return await _refillRequestsDAL.UpdateRefillRequest(refill);
        }
    }
}
