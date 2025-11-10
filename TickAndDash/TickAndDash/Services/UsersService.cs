using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Enums;
using TickAndDash.Services.ServicesDtos;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class UsersService : IUsersService
    {

        private readonly IUsersDAL _userDAL;
        private readonly IRidersDAL _ridersDAL;
        private readonly IDriversDAL _driversDAL;
        private readonly IConfiguration _configuration;
        private readonly IUsersSessionsDAL _usersSessionsDAL;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IMapper _mapper;

        public UsersService(IUsersDAL userDAL, IRidersDAL ridersDAL, IDriversDAL driversDAL, IConfiguration configuration, IUsersSessionsDAL usersSessionsDAL, IActionContextAccessor actionContextAccessor, IMapper mapper)
        {
            _userDAL = userDAL;
            _ridersDAL = ridersDAL;
            _driversDAL = driversDAL;
            _configuration = configuration;
            _usersSessionsDAL = usersSessionsDAL;
            _actionContextAccessor = actionContextAccessor;
            _mapper = mapper;
        }

        public async Task<Riders> GetRiderByMobileNumberAsync(string mobileNumber)
        {
            return await _ridersDAL.GetRiderByMobileNumberAsync(mobileNumber);
        }

        public async Task<User> GetUserAsync(int id, bool isActive)
        {
            return await _userDAL.GetUserAsync(id, isActive);
        }


        public async Task UpdateRiderGeneratedPincodeAsync(Riders riders)
        {
            await _ridersDAL.UpdateRiderGeneratedPincodeAsync(riders); ;
        }

        public bool UpdateRiderLoginInfo(Riders rider)
        {
            return _ridersDAL.UpdateRiderLoginInfo(rider); ;
        }


        private string GeneratRefrashToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, bool isActive)
        {
            return await _userDAL.UpdateUserAsync(userId, isActive);
        }


        public async Task<int> AddRiderAsync(Riders rider)
        {
            rider.User.RoleId = (int)RolesEnum.Rider;
            rider.User.IsActive = true;
            rider.User.CreationDate = DateTime.Now;
            rider.GeneratedPincode = Helpers.GenerateRandomPinCode();

            int userId = await _userDAL.AddUserAsync(rider.User);

            if (userId < 1)
            {
                throw new Exception("Fail to insert the user into the database");
            }

            rider.UserId = userId;
            bool isAdded = _ridersDAL.AddRiders(rider);

            if (isAdded == false)
            {
                throw new Exception("Fail to insert the rider into the database");
            }

            return userId;
        }

        //private bool UpdateUserRefreshToken(Users user)
        //{
        //    return _userDAL.UpdateUserRefreshToken(user);
        //}

        public async Task<Riders> GetRiderByIdAsync(int id)
        {
            return await _ridersDAL.GetRiderByIdAsync(id);
        }
        public async Task<Driver> GetDriverByLicenseNumberAsync(string licenseNumber)
        {
            return await _driversDAL.GetDriverByLicenseNumberAsync(licenseNumber);
        }

        public bool IsCorrectPassword(string password, string userPassowrd)
        {
            // password hased
            var hashedPassword = GetHash(password);
            return hashedPassword == userPassowrd ? true : false;
        }



        public async Task<bool> UpdateRiderInfoAsync(int userId, string Name, char gender)
        {
            await _userDAL.UpdateUserAsync(userId, Name);
            await _userDAL.UpdateRiderAsync(userId, gender);
            return true;
        }

        public AuthDto DriversLogin(DriversLoginRequest driversLoginRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateDriverInfoAsync(int userId, string Name, string password)
        {
            bool isUserUpdated = await _userDAL.UpdateUserAsync(userId, Name);
            bool success = true;

            if (!isUserUpdated)
            {
                success = false;
            }



            if (!string.IsNullOrWhiteSpace(password))
            {
                string hashpassword = GetHash(password);

                bool isPasswordUpdated = _userDAL.UpdateDriverPassword(userId, hashpassword);

                if (!isPasswordUpdated)
                {
                    success = false;
                }
            }


            return success;
        }

        public async Task<bool> IsDriverPasswordCorrectAsync(int userId, string password)
        {
            var driver = await _driversDAL.GetDriverByUserIdAsync(userId);
            return IsCorrectPassword(password, driver.Password);
        }

        //Change new List<> to list.Add
        public string CreateUserToken(object user, string fcm)
        {
            var claims = new List<Claim>();
            var httpContext = _actionContextAccessor.ActionContext.HttpContext;

            if (user is Riders)
            {
                var rider = user as Riders;
                claims = new List<Claim>
                {
                  new Claim (ClaimsEnum.MobileNumber.ToString(), rider.MobileNumber),
                  new Claim(ClaimsEnum.Pincode.ToString(), rider.GeneratedPincode.ToString()),
                  new Claim(ClaimsEnum.UserId.ToString(), rider.UserId.ToString()),
                  new Claim(ClaimTypes.Role, "Rider"),
                   new Claim(ClaimsEnum.FCM.ToString(), fcm)
                };

            }
            else if (user is Driver)
            {
                var driver = user as Driver;
                claims = new List<Claim>
                {
                  new Claim (ClaimsEnum.MobileNumber.ToString(), driver.MobileNumber),
                  new Claim (ClaimsEnum.LicenseNumber.ToString(), driver.LicenseNumber),
                  new Claim(ClaimsEnum.CarId.ToString(), driver.CarId.ToString()),
                  new Claim(ClaimsEnum.UserId.ToString(), driver.UserId.ToString()),
                  new Claim(ClaimTypes.Role, "Driver"),
                  new Claim(ClaimsEnum.FCM.ToString(), fcm)
                };
            }
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, CreateCryptographicallyStrongGuid().ToString()));
            claims.Add(new Claim(ClaimsEnum.UserAgnet.ToString(), httpContext.Request.Headers?["User-Agent"].ToString()));
            claims.Add(new Claim(ClaimsEnum.IP.ToString(), httpContext.Connection.RemoteIpAddress?.ToString()));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddMinutes(32);// 5 // We may consider expiring the token after 7 days or something like this 

            var t = new JwtSecurityToken(
                _configuration["Issuer"],
                _configuration["Audience"],
                claims,
                //expires: expires,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(t);
        }


        public async Task<bool> AddUserSessionAsync(int userId, DeviceInfo deviceInfo)
        {
            var userSession = _mapper.Map<UsersSessions>(deviceInfo);
            userSession.UserId = userId;
            userSession.LogingDate = DateTime.Now;
            return await _usersSessionsDAL.AddUserSessionAsync(userSession);
        }
        public AuthDto CreateRiderToken(Riders rider, string userAgnet, string ip)
        {
            AuthDto authDto = new AuthDto();

            var claims = new List<Claim>
                {
                    new Claim (ClaimsEnum.MobileNumber.ToString(), rider.MobileNumber),
                    new Claim(JwtRegisteredClaimNames.Jti, CreateCryptographicallyStrongGuid().ToString()),
                    new Claim("Pincode", rider.GeneratedPincode.ToString()),
                    new Claim("", rider.UserId.ToString()),
                    new Claim("UserAgnet", userAgnet),
                    new Claim("IP", ip),
                    new Claim(ClaimTypes.Role, "Rider")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddMinutes(32);// 5

            var t = new JwtSecurityToken(
                _configuration["Issuer"],
                _configuration["Audience"],
                claims,
                //expires: expires,
                signingCredentials: creds
                );

            //rider.User.RefreshToken = GeneratRefrashToken();
            // No Need for it
            //UpdateUserRefreshToken(rider.User);
            // UpdateRideLoginStatus(rider.UserId, true);
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(t);
            //authDto.RefreshToken = rider.User.RefreshToken;

            return authDto;

        }

        public AuthDto CreateDriverToken(Driver drivers, string userAgnet, string ip)
        {
            AuthDto authDto = new AuthDto();

            var claims = new List<Claim>
                {
                    new Claim ("LicenseNumber", drivers.LicenseNumber),
                    new Claim(JwtRegisteredClaimNames.Jti, CreateCryptographicallyStrongGuid().ToString()),
                    new Claim("CarId", drivers.CarId.ToString()),
                    new Claim("UserId", drivers.UserId.ToString()),
                    new Claim("UserAgnet", userAgnet),
                    new Claim("IP", ip),
                    new Claim(ClaimTypes.Role, "Driver")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddMinutes(32);// 5

            var t = new JwtSecurityToken(
                _configuration["Issuer"],
                _configuration["Audience"],
                claims,
                //expires: expires,
                signingCredentials: creds
                );

            authDto.Token = new JwtSecurityTokenHandler().WriteToken(t);

            return authDto;
        }
        private Guid CreateCryptographicallyStrongGuid()
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var data = new byte[16];
            rng.GetBytes(data);
            return new Guid(data);
        }

        private string GetHash(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            byte[] salt = new byte[128 / 8];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public bool UpdateUser(int userId, DeviceInfo deviceInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserSessionLogoutDatetimeAsync(int userId)
        {
            return await _usersSessionsDAL.UpdateUserSessionLogoutAsync(userId, DateTime.Now);
        }

        public async Task<bool> UpdateUserFCMTokenAsync(int userId, string FCMToken)
        {
            return await _userDAL.UpdateUserFCMTokenAsync(userId, FCMToken);
        }

        public async Task<bool> UpdateUserTokenAsync(int userId, string token)
        {
            return await _userDAL.UpdateUserTokenAsync(userId, token);
        }

        public async Task<bool> UpdateUserLanguageAsync(int userId, string language)
        {
            return await _userDAL.UpdateUserLanguageAsync(userId, language);
        }

        public async Task<bool> UpdateRiderAsync(Riders rider)
        {
            return await _ridersDAL.UpdateRiderAsync(rider);
        }

        public async Task<string> GetUserLanguageAsync(int userId)
        {
            return await _userDAL.GetUserLanguageAsync(userId);
        }

        public async Task<bool> IsDriverActiveAsync(int driverId)
        {
            return await _driversDAL.IsDriverActiveAsync(driverId);

        }

        public async Task<(int, string, string)> GetUserIdByMobileNumberAsync(string mobileNumber, RolesEnum roles)
        {
            int userId;
            string fcm = "";
            string language = "";
            if (roles == RolesEnum.Driver)
            {
                userId = await _driversDAL.GetDriverUserIdByMobileNumberAsync(mobileNumber);
                var driver = await _driversDAL.GetDriverByUserIdAsync(userId);
                fcm = driver?.User.FCMToken;
                language = driver?.User.Language;
            }

            if (roles == RolesEnum.Rider)
            {
                var user = await _ridersDAL.GetRiderByMobileNumberAsync(mobileNumber);
                userId = user != null ? user.UserId : 0;
                fcm = user?.User.FCMToken;
                language = user?.User.Language;
            }
            else
            {
                userId = await _driversDAL.GetDriverUserIdByMobileNumberAsync(mobileNumber);
                if (userId < 1)
                {
                    var user = await _ridersDAL.GetRiderByMobileNumberAsync(mobileNumber);
                    userId = user != null ? user.UserId : 0;
                    fcm = user?.User.FCMToken;
                    language = user?.User.Language;
                }
                else
                {
                    var driver = await _driversDAL.GetDriverByUserIdAsync(userId);
                    fcm = driver?.User.FCMToken;
                    language = driver?.User.Language;
                }
            }

            return (userId, fcm, language);
        }

        public async Task<Driver> GetDriverByUserIdAsync(int userId)
        {
            return await _driversDAL.GetDriverByUserIdAsync(userId);
        }

        public async Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS)
        {
            return await _driversDAL.UpdateDriverMobileOs(driverId, mobileOS);

        }

        public async Task<Driver> GetDriverByCarIdAsync(int carId)
        {
            return await _driversDAL.GetDriverByCarIdAsync(carId);
        }

        public async Task<User> GetManualTicketUserByNameAsync(string manulat)
        {
            return await _userDAL.GetByNameAsync("ManualTicket");
        }

        public async Task<int> CreateManualTicketUserAsync()
        {
            return await _userDAL.CreateManualTicketUserAsync();
        }

        public async Task<int> GetCountOfPinCodesGeneratedAsync(string mobileNumber)
        {
            return await _userDAL.GetCountOfPinCodesGeneratedAsync(mobileNumber);
        }
    }
}
