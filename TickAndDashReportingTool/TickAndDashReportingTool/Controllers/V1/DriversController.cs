using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Services.Interfaces;


namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    //[Authorize]
    public class DriversController : ControllerBase
    {
        private readonly IDriversService _driversService;
        private readonly IUsersService _usersService;

        public DriversController(IDriversService driversService, IUsersService usersService)
        {
            _driversService = driversService;
            _usersService = usersService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Driver> drivers = _driversService.GetAllDrivers();
            drivers.ForEach(d =>
            {
                d.Password = "";
                d.Token = "";
                d.User.Token = ""; ;
                d.User.FCMToken = "";
            });

            return Ok(drivers);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateDriverRequest createDriverRequest)
        {
            bool result = false;

            try
            {
                createDriverRequest.DriverName = createDriverRequest.LicenseNumber;

                var isMobileExist = await _usersService.IsMobileNumberExist($"972{createDriverRequest.MSISDN.Substring(createDriverRequest.MSISDN.Trim().Length - 9)}");

                if (isMobileExist)
                {
                    return BadRequest(new
                    {
                        messageAr = "عذرًا، رقم الموبايل  المدخل لديه حساب مسجل",
                        messageEn = "Sorry, mobile number has active account"
                    });
                }


                var driver = await _driversService.GetDriverBylicenseNumberAsync(createDriverRequest.LicenseNumber);
                if (driver != null)
                {
                    return BadRequest(new
                    {
                        messageAr = "عذرًا، رقم الرخصة موجود مسبقًا",
                        messageEn = "Sorry, LicenseNumber already exist"
                    });
                }

                result = await _driversService.CreateUserAsync(createDriverRequest);
            }
            catch (HttpStatusException ex)
            {
                return BadRequest(new
                {
                    messageAr = "عذرًا، يرجى التاكد من الرقم المدخل.",
                    messageEn = "Sorry, please check the entered mobile number."
                });
            }
            catch (Exception)
            {
                result = false;
            }

            if (result)
                return Ok(result);

            return BadRequest();
        }

        [HttpPut("{userId}")]
        public IActionResult Put(int userId, [FromBody] UpdateDriverRequest updateDriver)
        {
            bool result = _driversService.UpdateDriver(userId, updateDriver);
            return Ok(result);
        }

        // DELETE api/<DriversController>/5
        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            bool result = _driversService.DeleteDriver(userId);

            return Ok(result);
        }
    }
}
