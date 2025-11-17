using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                System.Diagnostics.Debug.WriteLine($"Login endpoint called. Username: {loginUserRequest?.Username ?? "null"}");
                
                if (loginUserRequest == null || string.IsNullOrWhiteSpace(loginUserRequest.Username) || string.IsNullOrWhiteSpace(loginUserRequest.Password))
                {
                    return BadRequest(new { StatusCode = 400, Success = false, Message = "Username and Password are required" });
                }

                System.Diagnostics.Debug.WriteLine("Calling _userService.Login...");
                var result = _userService.Login(loginUserRequest);
                System.Diagnostics.Debug.WriteLine($"Login result: {(result != null ? "not null" : "null")}");

                if (result != null)
                {
                    return Ok(result);
                }

                return Unauthorized(new { StatusCode = 401, Success = false, Message = "Invalid username or password" });
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"ArgumentException in Login: {argEx.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {argEx.StackTrace}");
                return BadRequest(new { StatusCode = 400, Success = false, Message = argEx.Message });
            }
            catch (UnauthorizedAccessException authEx)
            {
                System.Diagnostics.Debug.WriteLine($"UnauthorizedAccessException in Login: {authEx.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {authEx.StackTrace}");
                return Unauthorized(new { StatusCode = 401, Success = false, Message = authEx.Message });
            }
            catch (InvalidOperationException opEx)
            {
                System.Diagnostics.Debug.WriteLine($"InvalidOperationException in Login: {opEx.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {opEx.StackTrace}");
                if (opEx.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InnerException: {opEx.InnerException.Message}");
                }
                return StatusCode(500, new { StatusCode = 500, Success = false, Message = opEx.Message, InnerException = opEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in Login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { StatusCode = 500, Success = false, Message = $"An error occurred: {ex.Message}", InnerException = ex.InnerException?.Message, StackTrace = ex.StackTrace });
            }
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
