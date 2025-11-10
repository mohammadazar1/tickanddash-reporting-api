using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;

namespace TickAndDash.Services.Interfaces
{
    public interface ICarService
    {
        Task<int> GetCarActiveDriverIdByCarIdAsync(int id);
        Task<int> GetCarDriversCountAsync(int carId);
        Task<int> GetCarIdByCarCodeAsync(string carCode);
        Task<int> GetCarCountOfSeatsAsync(int carId);
        Task<bool> UpdateLoggedInCarUserAsync(int carId, int driverId);
        Task<bool> SwtichLoggedInCarUserAsync(int carId, int driverId);

        Task<bool> IsCarActiveAsync(int carId);
        Task<bool> IsCarActiveByCarCodeAsync(string carCode);
    }
}
