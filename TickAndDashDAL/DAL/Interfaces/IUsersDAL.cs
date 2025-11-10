using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IUsersDAL
    {
        Task<int> AddUserAsync(User user);

        Task<User> GetUserAsync(int id, bool isActive);

        //Task<User> GetUserByMsisdnAsync(string msisdn);

        //Users GetUserByMobileNumber(Users users);

        Task<Riders> GetRiderByMsisdnAsync(string msisdn);

        Task<User> GetDriverByMsisdn(string msisdn);

        Task<User> GetUserAsync(string mobileNumber, bool isActive);
        Task<string> GetUserLanguageAsync(int userId);
        Task<User> GetByNameAsync(string name);


        Task<bool> UpdateUserLanguageAsync(int userId, string language);

        Task<bool> UpdateUserAsync(int userId, bool isActive);
        Task<bool> UpdateUserAsync(int userId, string Name);
        Task<bool> UpdateUserFCMTokenAsync(int userId, string FCMToken);


        Task<bool> UpdateRiderAsync(int userId, char gender);
        Task<bool> UpdateUserTokenAsync(int userId, string token);
        bool UpdateDriverPassword(int userId, string password);
        Task<int> CreateManualTicketUserAsync();
        Task<int> GetCountOfPinCodesGeneratedAsync(string mobileNumber);
        //bool UpdateUserRefreshToken(Users user);
    }
}
