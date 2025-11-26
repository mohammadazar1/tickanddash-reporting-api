using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Helpers;
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

        public async Task<bool> CreateUserAsync(CreateDriverRequest createDriverRequest)
        {
            // نطبع MSISDN إلى شكل 972XXXXXXXXX (آخر 9 أرقام)
            var msisdn = $"972{createDriverRequest.MSISDN.Substring(createDriverRequest.MSISDN.Trim().Length - 9)}";

            // 1) نطلب من DigitalCodex التحقق من الرقم وإرجاع Token / Address
            var requestData = new RequestData
            {
                MSISDN = msisdn,
                Channel = 10,
                ChannelType = 11,
                PIN = createDriverRequest.MSISDN
            };

            var digitalCodexRes = await _digitalCodexClient.ValidateNumberAsync(requestData, "en");

            if (!digitalCodexRes.Success)
            {
                // الـ Controller رح يتعامل مع false ويرجع 400 برسالة مفهومة
                return false;
            }

            // 2) نبني كائن Driver كامل، فيه User بالداخل
            var driver = new Driver
            {
                Address = string.IsNullOrWhiteSpace(digitalCodexRes.Data.Address)
                            ? createDriverRequest.Address
                            : digitalCodexRes.Data.Address,
                Token = digitalCodexRes.Data.Token,
                MobileNumber = msisdn,
                Password = createDriverRequest.Password.Hash(),
                CarId = createDriverRequest.CarId,
                LicenseNumber = createDriverRequest.LicenseNumber,
                User = new User
                {
                    Name = createDriverRequest.DriverName,
                    RoleId = (int)RolesEnum.Driver
                }
            };

            // 3) DAL يتكفّل بإنشاء صف في Users و Drivers
            return _driverDAL.Insert(driver);
        }

        public List<Driver> GetAllDrivers()
        {
            return _driverDAL.GetDrivers();
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

        public async Task<Driver> GetDriverBylicenseNumberAsync(string licenseNumber)
        {
            return await _driverDAL.GetDriverByLicenseNumberAsync(licenseNumber);
        }
    }
}
