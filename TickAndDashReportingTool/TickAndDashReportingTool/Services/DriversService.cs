using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Helpers;
using TickAndDashReportingTool.HttpClients.DigitalCodex;              // فيه RegisterUserDto و DigitalCodexResponseDto
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;

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
            // نفس طريقة تكوين رقم الجوال الموجودة في الكنترولر
            var msisdn = $"972{createDriverRequest.MSISDN.Substring(createDriverRequest.MSISDN.Trim().Length - 9)}";

            // هذا هو DTO الحقيقي الموجود في:
            // HttpClients/DigitalCodex/DTOs/RegisterUserDto.cs
            var registerUser = new RegisterUserDto
            {
                // الخصائص الباقية إلها default داخل الكلاس
                MSISDN   = msisdn,
                Name     = createDriverRequest.DriverName,
                UserName = msisdn,
                Password = createDriverRequest.Password,
                Location = createDriverRequest.Address
            };

            // نستعمل RegisterUserAsync بدل ValidateNumberAsync
            var digitalCodexRes = await _digitalCodexClient.RegisterUserAsync(registerUser);

            if (digitalCodexRes == null || !digitalCodexRes.Success || digitalCodexRes.Data == null)
                return false;

            // على حسب الكلاسات الموجودة عادة:
            // DigitalCodexRegisterUserResponse فيه Token و Address
            var dcData = digitalCodexRes.Data;

            var driver = new Driver
            {
                Address = string.IsNullOrWhiteSpace(dcData.Address)
                            ? createDriverRequest.Address
                            : dcData.Address,
                Token         = dcData.Token,
                MobileNumber  = msisdn,
                Password      = createDriverRequest.Password.Hash(),
                CarId         = createDriverRequest.CarId,
                LicenseNumber = createDriverRequest.LicenseNumber,
                User = new User
                {
                    Name   = createDriverRequest.DriverName,
                    RoleId = 3   // Driver
                }
            };

            return _driverDAL.Insert(driver);
        }
    }
}
