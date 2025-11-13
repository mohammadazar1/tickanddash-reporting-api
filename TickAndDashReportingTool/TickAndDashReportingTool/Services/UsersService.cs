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
                // Validate input
                if (loginUserRequest == null)
                {
                    throw new ArgumentNullException(nameof(loginUserRequest), "Login request cannot be null");
                }

                if (string.IsNullOrWhiteSpace(loginUserRequest.Username))
                {
                    throw new ArgumentException("Username cannot be null or empty", nameof(loginUserRequest.Username));
                }

                if (string.IsNullOrWhiteSpace(loginUserRequest.Password))
                {
                    throw new ArgumentException("Password cannot be null or empty", nameof(loginUserRequest.Password));
                }

                // Ensure password is not null before hashing
                var passwordToHash = loginUserRequest.Password?.Trim();
                if (string.IsNullOrWhiteSpace(passwordToHash))
                {
                    throw new ArgumentException("Password cannot be null or empty after trim", nameof(loginUserRequest.Password));
                }

                // Ensure username is not null before trimming
                var usernameToSearch = loginUserRequest.Username?.Trim();
                if (string.IsNullOrWhiteSpace(usernameToSearch))
                {
                    throw new ArgumentException("Username cannot be null or empty after trim", nameof(loginUserRequest.Username));
                }

                Admin admin = null;
                try
                {
                    admin = _adminDAL.GetByUserName(usernameToSearch);
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
                        throw new InvalidOperationException("Admin password is not set");
                    }

                    // Hash the password for comparison
                    string hashedPassword = null;
                    try
                    {
                        hashedPassword = passwordToHash.Hash();
                    }
                    catch (Exception hashEx)
                    {
                        throw new InvalidOperationException($"Failed to hash password: {hashEx.Message}", hashEx);
                    }

                    // Compare passwords
                    if (hashedPassword == null || hashedPassword != admin.Password)
                    {
                        throw new UnauthorizedAccessException("Invalid username or password");
                    }

                    // Create token
                    try
                    {
                        var authUser = new AuthUser
                        {
                            Username = admin.Username ?? "",
                            Id = admin.UserId,
                            Role = admin.Role ?? "Admin"
                        };

                        var token = CreateToken(authUser);
                        
                        return new
                        {
                            token = token,
                            role = admin.Role ?? "Admin"
                        };
                    }
                    catch (Exception tokenEx)
                    {
                        throw new InvalidOperationException($"Failed to create token: {tokenEx.Message}", tokenEx);
                    }
                }

                // Check POS
                var pos = (PointOfSales)null;
                try
                {
                    pos = _pointOfSalesDAL.GetPOSByUsername(usernameToSearch);
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
                        throw new InvalidOperationException("POS password is not set");
                    }

                    // Hash the password for comparison
                    string hashedPassword = null;
                    try
                    {
                        hashedPassword = passwordToHash.Hash();
                    }
                    catch (Exception hashEx)
                    {
                        throw new InvalidOperationException($"Failed to hash password: {hashEx.Message}", hashEx);
                    }

                    // Compare passwords
                    if (hashedPassword == null || hashedPassword != pos.Password)
                    {
                        throw new UnauthorizedAccessException("Invalid username or password");
                    }

                    // Create token
                    try
                    {
                        var authUser = new AuthUser
                        {
                            Username = pos.Username ?? "",
                            Id = pos.UserId,
                            Role = "POS"
                        };

                        var token = CreateToken(authUser);
                        
                        return new
                        {
                            token = token,
                            role = "POS"
                        };
                    }
                    catch (Exception tokenEx)
                    {
                        throw new InvalidOperationException($"Failed to create token: {tokenEx.Message}", tokenEx);
                    }
                }

                throw new UnauthorizedAccessException("Invalid username or password");
            }
            catch (Exception ex)
            {
                // Re-throw to be caught by controller/exception handler
                throw;
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
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                // Ensure user properties are not null
                var username = user.Username ?? "";
                var role = user.Role ?? "User";
                var userId = user.Id;

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                };

                // Get configuration values with fallbacks
                var keyString = "DefaultKeyForDevelopmentOnly12345678901234567890";
                if (_configuration != null)
                {
                    keyString = _configuration["Key"] 
                        ?? _configuration["Jwt:Key"] 
                        ?? _configuration["JWT:Key"]
                        ?? keyString;
                }

                var issuer = "TickAndDash";
                if (_configuration != null)
                {
                    issuer = _configuration["Issuer"] 
                        ?? _configuration["Jwt:Issuer"] 
                        ?? _configuration["JWT:Issuer"]
                        ?? issuer;
                }

                var audience = "TickAndDash";
                if (_configuration != null)
                {
                    audience = _configuration["Audience"] 
                        ?? _configuration["Jwt:Audience"] 
                        ?? _configuration["JWT:Audience"]
                        ?? audience;
                }

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

                // Validate keyString before encoding
                if (keyString == null)
                {
                    throw new InvalidOperationException("JWT Key cannot be null");
                }

                byte[] keyBytes;
                try
                {
                    keyBytes = Encoding.UTF8.GetBytes(keyString);
                }
                catch (Exception encodeEx)
                {
                    throw new InvalidOperationException($"Failed to encode JWT key: {encodeEx.Message}", encodeEx);
                }

                if (keyBytes == null || keyBytes.Length == 0)
                {
                    throw new InvalidOperationException("JWT key bytes cannot be null or empty");
                }

                var key = new SymmetricSecurityKey(keyBytes);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    signingCredentials: creds
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create JWT token: {ex.Message}", ex);
            }
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
