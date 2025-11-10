using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IDriversService
    {
        List<Driver> GetAllDrivers();
        Task<Driver> GetDriverBylicenseNumberAsync(string licenseNumber);

        Task<bool> CreateUserAsync(CreateDriverRequest createDriverRequest);
        bool DeleteDriver(int userId);
        bool UpdateDriver(int userId, UpdateDriverRequest updateDriver);
    }
}
