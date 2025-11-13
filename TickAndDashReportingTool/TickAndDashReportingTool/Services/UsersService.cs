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
            try
            {
                if (loginUserRequest == null || string.IsNullOrWhiteSpace(loginUserRequest.Username) || string.IsNullOrWhiteSpace(loginUserRequest.Password))
                {
                    return "";
                }

                Admin admin = null;
                try
                {
                    admin = _adminDAL.GetByUserName(loginUserRequest.Username);
                }
                catch
                {
                    // If GetByUserName fails, continue to check POS
                }

                if (admin != null)
                {
                    // Check if admin.Password is null or empty
                    if (string.IsNullOrWhiteSpace(admin.Password))
                    {
                        return "";
                    }

                    // Hash the password for comparison
                    string hashedPassword = null;
                    try
                    {
                        hashedPassword = loginUserRequest.Password.Hash();
                    }
                    catch
                    {
                        return "";
                    }

                    // Compare passwords
                    if (hashedPassword == null || hashedPassword != admin.Password)
                    {
                        return "";
                    }

                    // Create token
                    try
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
                    catch (Exception tokenEx)
                    {
                        // Token creation failed
                        return "";
                    }
                }

                // Check POS
                var pos = (PointOfSales)null;
                try
                {
                    pos = _pointOfSalesDAL.GetPOSByUsername(loginUserRequest.Username);
                }
                catch
                {
                    // If GetPOSByUsername fails, continue
                }

                if (pos != null)
                {
                    // Check if pos.Password is null or empty
                    if (string.IsNullOrWhiteSpace(pos.Password))
                    {
                        return "";
                    }

                    // Hash the password for comparison
                    string hashedPassword = null;
                    try
                    {
                        hashedPassword = loginUserRequest.Password.Hash();
                    }
                    catch
                    {
                        return "";
                    }

                    // Compare passwords
                    if (hashedPassword == null || hashedPassword != pos.Password)
                    {
                        return "";
                    }

                    // Create token
                    try
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
                    catch (Exception tokenEx)
                    {
                        // Token creation failed
                        return "";
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                // Log error but return empty string (don't expose error details)
                return "";
            }
        }

        public object CreateFirstAdmin(RegisterUserRequest registerUserRequest)
        {
            try
            {
                if (registerUserRequest == null || string.IsNullOrWhiteSpace(registerUserRequest.Username) || string.IsNullOrWhiteSpace(registerUserRequest.Password))
                {
                    return new { Success = false, Message = "Username and Password are required" };
                }

                // Check if any admin exists with this username
                Admin existingAdmin = null;
                try
                {
                    existingAdmin = _adminDAL.GetByUserName(registerUserRequest.Username);
                }
                catch
                {
                    // If GetByUserName fails, assume no admin exists (first admin)
                }

                if (existingAdmin != null)
                {
                    return new { Success = false, Message = "Admin with this username already exists" };
                }

                // Create admin
                var adminDto = new Admin
                {
                    MSISDN = registerUserRequest.Msisdn ?? "0000000000",
                    Password = string.IsNullOrWhiteSpace(registerUserRequest.Password) ? "" : registerUserRequest.Password.Hash(),
                    Username = registerUserRequest.Username ?? ""
                };

                if (string.IsNullOrWhiteSpace(adminDto.Username))
                {
                    return new { Success = false, Message = "Username cannot be empty" };
                }

                if (string.IsNullOrWhiteSpace(adminDto.Password))
                {
                    return new { Success = false, Message = "Password cannot be empty" };
                }

                bool insertResult = false;
                try
                {
                    insertResult = _adminDAL.Insert(adminDto);
                }
                catch (Exception insertEx)
                {
                    return new { 
                        Success = false, 
                        Message = $"Failed to insert admin: {insertEx.Message}",
                        InnerException = insertEx.InnerException?.Message
                    };
                }

                if (insertResult)
                {
                    // Wait a bit for the insert to complete and be queryable
                    System.Threading.Thread.Sleep(500);

                    // Try to login to get token
                    try
                    {
                        var loginRequest = new LoginUserRequest
                        {
                            Username = registerUserRequest.Username,
                            Password = registerUserRequest.Password
                        };

                        var loginResult = Login(loginRequest);
                        
                        if (loginResult != null)
                        {
                            var loginResultString = loginResult.ToString();
                            if (!string.IsNullOrWhiteSpace(loginResultString) && loginResultString != "")
                            {
                                return new { Success = true, Message = "First admin created successfully", Data = loginResult };
                            }
                        }
                    }
                    catch (Exception loginEx)
                    {
                        // Login failed but admin was created - return success with message
                        return new { 
                            Success = true, 
                            Message = "First admin created successfully. Please login using /api/report/Users/login",
                            LoginError = loginEx.Message
                        };
                    }

                    // Admin created but login returned empty - return success anyway
                    return new { Success = true, Message = "First admin created successfully. Please login using /api/report/Users/login" };
                }

                return new { Success = false, Message = "Failed to create admin - Insert returned false" };
            }
            catch (Exception ex)
            {
                return new { 
                    Success = false, 
                    Message = $"Error creating admin: {ex.Message}",
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                };
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

            var keyString = _configuration?["Key"] ?? _configuration?["Jwt:Key"] ?? "DefaultKeyForDevelopmentOnly12345678901234567890";
            var issuer = _configuration?["Issuer"] ?? _configuration?["Jwt:Issuer"] ?? "TickAndDash";
            var audience = _configuration?["Audience"] ?? _configuration?["Jwt:Audience"] ?? "TickAndDash";

            // Ensure keyString is not null or empty
            if (string.IsNullOrWhiteSpace(keyString))
            {
                keyString = "DefaultKeyForDevelopmentOnly12345678901234567890";
            }

            // Ensure issuer is not null or empty
            if (string.IsNullOrWhiteSpace(issuer))
            {
                issuer = "TickAndDash";
            }

            // Ensure audience is not null or empty
            if (string.IsNullOrWhiteSpace(audience))
            {
                audience = "TickAndDash";
            }

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
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty");
            }

            var keyString = _configuration["Key"] ?? _configuration["Jwt:Key"] ?? "DefaultKeyForDevelopmentOnly12345678901234567890";
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString)),
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
