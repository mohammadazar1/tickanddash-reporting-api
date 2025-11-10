using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Services.ServicesDtos;
using TickAndDashDAL.DAL;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public interface IUsersService
    {

        Task<int> GetCountOfPinCodesGeneratedAsync(string mobileNumber);

        Task<(int,string, string)> GetUserIdByMobileNumberAsync(string mobileNumber, RolesEnum rolesEnum);
        Task<Riders> GetRiderByMobileNumberAsync(string mobileNumber);
        Task<Riders> GetRiderByIdAsync(int id);
        Task<Driver> GetDriverByLicenseNumberAsync(string licenseNumber);
        Task<Driver> GetDriverByUserIdAsync(int userId);

        Task<string> GetUserLanguageAsync(int userId);
        Task<User> GetUserAsync(int id, bool isActive);



        Task<int> AddRiderAsync(Riders riders);
        Task<bool> AddUserSessionAsync(int userId, DeviceInfo deviceInfo);

        Task<bool> UpdateRiderAsync(Riders riders);

        Task UpdateRiderGeneratedPincodeAsync(Riders riders);
        Task<bool> UpdateUserFCMTokenAsync(int userId, string FCMToken);
        Task<bool> UpdateUserTokenAsync(int userId, string token);

        bool UpdateRiderLoginInfo(Riders rider);

        Task<bool> IsDriverPasswordCorrectAsync(int userId, string password);

        Task<bool> IsDriverActiveAsync(int driverId);

        Task<bool> UpdateRiderInfoAsync(int userId, string Name, char gender);
        Task<bool> UpdateDriverInfoAsync(int userId, string Name, string password);

        Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS);
        //bool IsPasswordCorrect(int userId, string password);
        // used
        //Riders GetRegisteredRiders(string mobileNumber);
        //bool UpdateRiderLoginPincode(int userId, int loginPincode);
        //bool UpdateRideLoginStatus(int userId, bool isLoggedIn);
        //bool UpdateUserRefreshToken(Users users);

        Task<bool> UpdateUserAsync(int userId, bool isActive);
        Task<bool> UpdateUserLanguageAsync(int userId, string language);
        AuthDto DriversLogin(DriversLoginRequest driversLoginRequest);

        AuthDto CreateRiderToken(Riders rider, string userAgnet, string ip);
        AuthDto CreateDriverToken(Driver drivers, string userAgnet, string ip);
        string CreateUserToken(object user, string FCM);

        bool UpdateUser(int userId, DeviceInfo deviceInfo);
        Task<bool> UpdateUserSessionLogoutDatetimeAsync(int userId);
        Task<Driver> GetDriverByCarIdAsync(int carId);
        Task<User> GetManualTicketUserByNameAsync(string manulat);
        Task<int> CreateManualTicketUserAsync();
    }
}
