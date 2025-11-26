using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Helpers;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
using TickAndDashReportingTool.HttpClients.DigitalCodex.DTOs;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class DriversService : IDriversService
    {
        private readonly IDriversDAL _driverDAL;
        private readonly IDigitalCodexClient _digitalCodexClient;

        public DriversService(IDriversDAL driverDAL, IDigitalCodexClient digitalCodexClient)
        {
            _driverDAL = driverDAL;
            _digitalCodexClient = digitalCodexClient;
        }

        public List<Driver> GetAllDrivers()
        {
            return _driverDAL.GetDrivers();
        }

        public async Task<Driver> GetDriverBylicenseNumberAsync(string licenseNumber)
        {
            return await _driverDAL.GetDriverByLicenseNumberAsync(licenseNumber);
        }

        public bool UpdateDriver(int userId, UpdateDriverRequest updateDriver)
        {
            var driver = new Driver
            {
                UserId = userId,
                LicenseNumber = updateDriver.LicenseNumber,
                CarId = updateDriver.CarId,
                Address = updateDriver.Address,
                IsActive = updateDriver.IsActive
            };

            return _driverDAL.Update(driver);
        }

        public bool DeleteDriver(int userId)
        {
            return _driverDAL.Delete(userId);
        }

        public async Task<bool> CreateUserAsync(CreateDriverRequest createDriverRequest)
        {
            // صيغة رقم الهاتف
            var msisdn = $"972{createDriverRequest.MSISDN.Substring(createDriverRequest.MSISDN.Trim().Length - 9)}";

            // الـ DTO الصحيح
            var registerUser = new RegisterUserDto
            {
                MSISDN   = msisdn,
                Name     = createDriverRequest.DriverName,
                UserName = msisdn,
                Password = createDriverRequest.Password,
                Location = createDriverRequest.Address
            };

            // Call DigitalCodex
            var digitalCodexRes = await _digitalCodexClient.RegisterUserAsync(registerUser);

            if (digitalCodexRes == null || !digitalCodexRes.Success || digitalCodexRes.Data == null)
                return false;

            // Response يحتوي فقط Token + UserId + IsAutherized
            var dcData = digitalCodexRes.Data;

            // إنشاء السائق
            var driver = new Driver
            {
                Address = createDriverRequest.Address,
                Token = dcData.Token,
                MobileNumber = msisdn,
                Password = createDriverRequest.Password.Hash(),
                CarId = createDriverRequest.CarId,
                LicenseNumber = createDriverRequest.LicenseNumber,
                User = new User
                {
                    Name = createDriverRequest.DriverName,
                    RoleId = 3
                }
            };

            return _driverDAL.Insert(driver);
        }
    }
}
