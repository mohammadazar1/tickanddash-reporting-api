using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;

namespace TickAndDash.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(string to, string body,
        string title, string click_action, string category,
        string mobileType, RolesEnum role, int RiderId = 0, decimal Amount = 0);
    }
}
