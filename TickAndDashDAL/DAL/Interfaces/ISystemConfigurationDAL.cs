using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ISystemConfigurationDAL
    {
        Task<string> GetSettingValueByKeyAsync(SettingKeyEnum settingKeyEnum);
        Task<List<SystemConfiguration>> GetAllAsync();
        bool Update(SystemConfiguration systemConfiguration);

    }
}
