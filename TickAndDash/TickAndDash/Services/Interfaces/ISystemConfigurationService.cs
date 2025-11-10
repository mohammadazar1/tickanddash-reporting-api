using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface ISystemConfigurationService
    {
        Task<string> GetSettingValueByKeyAsync(SettingKeyEnum settingKey);
       
        Task<List<SystemConfiguration>> GetAllSystemConfigAsync();

        Task<bool> IsCancellingValidForRiderAsync(int riderId);
    }
}
