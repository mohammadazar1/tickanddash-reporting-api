using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IUsersSessionsDAL
    {
        Task<bool> AddUserSessionAsync(UsersSessions usersSessions);
        Task<bool> UpdateUserSessionLogoutAsync(int userId, DateTime logoutDate);

    }
}
