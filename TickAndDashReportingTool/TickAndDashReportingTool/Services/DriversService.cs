using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Helpers;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
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

        public async Task<bool> CreateUserAsync(CreateDriverRequest createDriverRequest)
        {
            if (createDriverRequest.Password == "" || createDriverRequest.Password == null)
                return false;

            var digitalCodexRes = await _digitalCodexClient.RegisterUserAsync(new RegisterUserDto
            {
                Name = $"{createDriverRequest.MSISDN.Trim()}@DPalCap",
                UserName = createDriverRequest.LicenseNumber.Trim(),
                Password = createDriverRequest.Password,
                Email = $"{createDriverRequest.MSISDN}@DPalCap",
                MSISDN = $"972{createDriverRequest.MSISDN.Trim().Substring(createDriverRequest.MSISDN.Length - 9)}",
                Location = createDriverRequest.Address,
                //CanUseBalance = true
            });

            if (digitalCodexRes == null || digitalCodexRes.Success != true ||
                digitalCodexRes.Data == null)
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "DC");


            return _driverDAL.Insert(new Driver
            {
                LicenseNumber = createDriverRequest.LicenseNumber,
                CarId = createDriverRequest.CarId,
                Password = createDriverRequest.Password.Hash(),
                Address = createDriverRequest.Address,
                Token = digitalCodexRes.Data.Token,
                MobileNumber = $"972{createDriverRequest.MSISDN.Trim().Substring(createDriverRequest.MSISDN.Length - 9)}",
                User = new TickAndDashDAL.Models.User { Name = createDriverRequest.DriverName }
            });
        }



        public bool DeleteDriver(int userId)
        {
            return _driverDAL.Delete(userId);
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
            if (updateDriver.Password == "" || updateDriver.Password == null)
                updateDriver.Password = "";
            else
                updateDriver.Password = updateDriver.Password.Hash();


            return _driverDAL.Update(new Driver
            {
                LicenseNumber = updateDriver.LicenseNumber,
                Password = updateDriver.Password,
                CarId = updateDriver.CarId,
                Address = updateDriver.Address,
                UserId = userId,
                IsActive = updateDriver.IsActive
            });
        }


    }
}
