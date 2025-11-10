using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly ISystemConfigurationDAL _systemConfigurationDAL;

        public SystemConfigurationService(ISystemConfigurationDAL systemConfigurationDAL)
        {
            _systemConfigurationDAL = systemConfigurationDAL;
        }

        public async Task<List<SystemConfiguration>> GetAllConfigurationAsync()
        {
            return await _systemConfigurationDAL.GetAllAsync();
        }

        public bool Update(UpdateSystemConfigRequest updateSystemConfigRequest)
        {
            return _systemConfigurationDAL
                .Update(
                new SystemConfiguration
                {
                    Id = updateSystemConfigRequest.Id,
                    SettingValue = updateSystemConfigRequest.SettingValue
                });
        }
    }
}
