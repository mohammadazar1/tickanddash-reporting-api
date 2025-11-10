using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDash.Services
{
    public class CarService : ICarService
    {
        private readonly ICarsDAL _carsDAL;

        public CarService(ICarsDAL carsDAL)
        {
            _carsDAL = carsDAL;
        }

        public async Task<int> GetCarActiveDriverIdByCarIdAsync(int id)
        {
            return await _carsDAL.GetCarActiveDriverIdByCarIdAsync(id);
        }


        public async Task<int> GetCarCountOfSeatsAsync(int carId)
        {
            return await _carsDAL.GetCarCountOfSeatsAsync(carId);
        }

        public async Task<int> GetCarDriversCountAsync(int carId)
        {
            return await _carsDAL.GetCarDriversCountAsync(carId);
        }

        public async Task<int> GetCarIdByCarCodeAsync(string carCode)
        {
            return await _carsDAL.GetCarIdByCarCodeAsync(carCode);
        }

        public async Task<bool> IsCarActiveAsync(int carId)
        {
            return await _carsDAL.IsCarActiveAsync(carId);
        }

        public async Task<bool> IsCarActiveByCarCodeAsync(string carCode)
        {
            return await _carsDAL.IsCarActiveByCarCodeAsync(carCode);
        }

        public async Task<bool> SwtichLoggedInCarUserAsync(int carId, int driverId)
        {
            var carDriverId = await _carsDAL.GetCarActiveDriverIdByCarIdAsync(carId);

            if (driverId == carDriverId)
            {
                return await _carsDAL.UpdateLoggedInCarUserAsync(carId, 0);
            }

            return true;
        }

        public async Task<bool> UpdateLoggedInCarUserAsync(int carId, int driverId)
        {
            return await _carsDAL.UpdateLoggedInCarUserAsync(carId, driverId);
        }
    }
}
