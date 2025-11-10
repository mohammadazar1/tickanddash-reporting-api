using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserRequest loginUserRequest)
        {
            var result = _userService.Login(loginUserRequest);

            if (result != "")
            {
                return Ok(result);
            }

            return NotFound();
        }


        [Authorize]
        [Authorize(Roles = "Admin, Supervisor")]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserRequest registerUserRequest)
        {
            string token = _userService.Register(registerUserRequest);
            return Ok(token);
        }
    }
}
