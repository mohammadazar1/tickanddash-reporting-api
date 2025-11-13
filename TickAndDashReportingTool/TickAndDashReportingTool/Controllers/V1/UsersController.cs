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


        [HttpPost("create-first-admin")]
        public IActionResult CreateFirstAdmin([FromBody] RegisterUserRequest registerUserRequest)
        {
            var result = _userService.CreateFirstAdmin(registerUserRequest);
            
            // Check if result is anonymous object with Success property
            var resultType = result.GetType();
            var successProperty = resultType.GetProperty("Success");
            
            if (successProperty != null)
            {
                var successValue = successProperty.GetValue(result);
                if (successValue != null && (bool)successValue)
                {
                    return Ok(result);
                }
            }
            
            return BadRequest(result);
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
