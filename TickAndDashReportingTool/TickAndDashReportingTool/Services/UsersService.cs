using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Helpers;
using TickAndDashReportingTool.Services.Interfaces;
using TickAndDashReportingTool.Services.Models;

namespace TickAndDashReportingTool.Services
{
    public class UsersService : IUsersService
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminDAL _adminDAL;
        private readonly IUsersDAL _usersDAL;
        private readonly IPointOfSalesDAL _pointOfSalesDAL;



        public UsersService(IConfiguration configuration, IAdminDAL adminDAL, IUsersDAL usersDAL, IPointOfSalesDAL pointOfSalesDAL)
        {
            _configuration = configuration;
            _adminDAL = adminDAL;
            _usersDAL = usersDAL;
            _pointOfSalesDAL = pointOfSalesDAL;
        }


        public object Login(LoginUserRequest loginUserRequest)
        {
            if (loginUserRequest == null || string.IsNullOrWhiteSpace(loginUserRequest.Username) || string.IsNullOrWhiteSpace(loginUserRequest.Password))
            {
                return "";
            }

            var admin = _adminDAL.GetByUserName(loginUserRequest.Username);
            var pos = _pointOfSalesDAL.GetPOSByUsername(loginUserRequest.Username);

            if (admin != null)
            {
                if (string.IsNullOrWhiteSpace(loginUserRequest.Password) || loginUserRequest.Password.Hash() != admin.Password)
                {
                    return "";
                }
                else
                {
                    return new
                    {
                        token = CreateToken(new AuthUser
                        {
                            Username = admin.Username ?? "",
                            Id = admin.UserId,
                            Role = admin.Role ?? "Admin"
                        }),
                        role = admin.Role ?? "Admin"
                    };
                }
            }

            if (pos != null)
            {
                if (string.IsNullOrWhiteSpace(loginUserRequest.Password) || loginUserRequest.Password.Hash() != pos.Password)
                {
                    return "";
                }
                else
                {
                    return new
                    {
                        token = CreateToken(new AuthUser
                        {
                            Username = pos.Username ?? "",
                            Id = pos.UserId,
                            Role = "POS"
                        }),
                        role = "POS"
                    };
                }
            }

            return "";
        }

        public object CreateFirstAdmin(RegisterUserRequest registerUserRequest)
        {
            if (registerUserRequest == null || string.IsNullOrWhiteSpace(registerUserRequest.Username) || string.IsNullOrWhiteSpace(registerUserRequest.Password))
            {
                return new { Success = false, Message = "Username and Password are required" };
            }

            // Check if any admin exists
            var existingAdmin = _adminDAL.GetByUserName(registerUserRequest.Username);
            if (existingAdmin != null)
            {
                return new { Success = false, Message = "Admin with this username already exists" };
            }

            // Check if there are any admins at all
            // If no admins exist, allow creating first admin
            try
            {
                var adminDto = new Admin
                {
                    MSISDN = registerUserRequest.Msisdn ?? "0000000000",
                    Password = registerUserRequest.Password.Hash(),
                    Username = registerUserRequest.Username
                };

                if (_adminDAL.Insert(adminDto))
                {
                    var loginRequest = new LoginUserRequest
                    {
                        Username = registerUserRequest.Username,
                        Password = registerUserRequest.Password
                    };

                    var loginResult = Login(loginRequest);
                    return new { Success = true, Message = "First admin created successfully", Data = loginResult };
                }

                return new { Success = false, Message = "Failed to create admin" };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = $"Error creating admin: {ex.Message}" };
            }
        }

        public string Register(RegisterUserRequest registerUserRequest)
        {
            var adminDto = new Admin
            {
                MSISDN = registerUserRequest.Msisdn,
                Password = registerUserRequest.Password.Hash(),
                Username = registerUserRequest.Username
            };

            if (_adminDAL.Insert(adminDto))
            {
                return CreateToken(new AuthUser { Username = registerUserRequest.Username, Id = 1 });
            }

            return ""; // Should return Exception
        }

        private string CreateToken(AuthUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.UniqueName, user.Username ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var keyString = _configuration["Key"] ?? _configuration["Jwt:Key"] ?? "DefaultKeyForDevelopmentOnly12345678901234567890";
            var issuer = _configuration["Issuer"] ?? _configuration["Jwt:Issuer"] ?? "TickAndDash";
            var audience = _configuration["Audience"] ?? _configuration["Jwt:Audience"] ?? "TickAndDash";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(30);

            var t = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(t);
        }

        private string GenerateRefrashToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private Object RefreshOldToken()
        {
            return "";
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<bool> IsMobileNumberExist(string msisdn)
        {
            //_usersDAL.GetUserByMsisdnAsync(msisdn);

            bool isExist = true;

            var rider = await _usersDAL.GetRiderByMsisdnAsync(msisdn);

            if (rider == null)
            {
                var driver = await _usersDAL.GetDriverByMsisdn(msisdn);

                if (driver == null)
                {
                    isExist = false;
                }
            }

            return isExist;
        }

        public async Task<Riders> GetRiderByMsisdnAsync(string msisdn)
        {
            return await _usersDAL.GetRiderByMsisdnAsync(msisdn);
        }
    }
}
