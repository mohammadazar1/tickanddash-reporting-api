using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IDriversDAL
    {
        List<Driver> GetDrivers();

        Task<int> GetDriverUserIdByMobileNumberAsync(string mobileNumber);
        Task<Driver> GetDriverByLicenseNumberAsync(string licenseNumber);
        Task<Driver> GetDriverByUserIdAsync(int userId);
        Task<Driver> GetDriverByCarIdAsync(int carId);

        Task<bool> IsDriverActiveAsync(int driverId);

        bool Insert(Driver driver);
        bool Update(Driver driver);
        bool Delete(int userId);
        Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS);
    }
}
