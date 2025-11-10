using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ICarsDAL
    {
        Task<int> GetCarIdByCarCodeAsync(string carCode);
        Task<int> GetCarActiveDriverIdByCarIdAsync(int id);
        Task<int> GetCarDriversCountAsync(int carId);
        Task<int> GetCarCountOfSeatsAsync(int carId);
        Task<bool> IsCarActiveAsync(int carId);
        Task<bool> IsCarActiveByCarCodeAsync(string carCode);
        Task<bool> UpdateLoggedInCarUserAsync(int carId, int driverId);
        bool Create(Car car);
        bool Update(Car car);
        List<Car> GetAll();
        bool DeleteCar(int id);
        bool Insert(Car car);

    }
}
