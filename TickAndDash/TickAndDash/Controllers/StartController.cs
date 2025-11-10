using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StartupController : ControllerBase
    {
        private readonly IDriversDAL _driversDAL;

        public StartupController(IDriversDAL driversDAL)
        {
            _driversDAL = driversDAL;
        }

        [HttpGet]
        public IActionResult start()
        {
           var x = _driversDAL.GetDrivers();

            return Ok(x);
            return Ok(new { success = "ok", message = "Tick And dash starts"  });
        }

    }
}
