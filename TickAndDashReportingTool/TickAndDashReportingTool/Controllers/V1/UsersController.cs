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
            if (loginUserRequest == null || string.IsNullOrWhiteSpace(loginUserRequest.Username) || string.IsNullOrWhiteSpace(loginUserRequest.Password))
            {
                return BadRequest(new { StatusCode = 400, Success = false, Message = "Username and Password are required" });
            }

            var result = _userService.Login(loginUserRequest);

            if (result != null && result.ToString() != "")
            {
                return Ok(result);
            }

            return NotFound(new { StatusCode = 404, Success = false, Message = "Invalid username or password" });
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
