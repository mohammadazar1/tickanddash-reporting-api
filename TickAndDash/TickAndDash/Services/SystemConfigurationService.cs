using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly ISystemConfigurationDAL _ISystemConfigurationService;
        //private readonly IRidersQueueService _ridersQueueService;
        private readonly IQueuesService _queuesService;


        public SystemConfigurationService(ISystemConfigurationDAL iSystemConfigurationService , IQueuesService queuesService/*IRidersQueueService ridersQueueService*/)
        {
            _ISystemConfigurationService = iSystemConfigurationService;
            //_ridersQueueService = ridersQueueService;
           _queuesService = queuesService;
        }

        public async Task<List<SystemConfiguration>> GetAllSystemConfigAsync()
        {
            return  await _ISystemConfigurationService.GetAllAsync();
        }

        public async Task<string> GetSettingValueByKeyAsync(SettingKeyEnum settingKey)
        {
            return  await _ISystemConfigurationService.GetSettingValueByKeyAsync(settingKey);
        }


        public async Task<bool> IsCancellingValidForRiderAsync(int riderId)
        {
            bool success = true;

            int dBookingLimit = int.Parse(await GetSettingValueByKeyAsync(SettingKeyEnum.DCancellBookingCount));

            //int riderDCancellationCount = await _ridersQueueService.GetRaiderDailyCancellationCountAsync(riderId);
            int riderDCancellationCount = await _queuesService.GetRaiderDailyCancellationCountAsync(riderId);

            if (riderDCancellationCount >= dBookingLimit)
            {
                success = false;
            }
            else
            {
                int wBookingLimit = int.Parse(await GetSettingValueByKeyAsync(SettingKeyEnum.WCancellBookingCount));
                int riderWCancellationCount = await _queuesService.GetRaiderWeeklyCancellationCountAsync(riderId);

                if (riderWCancellationCount  >= wBookingLimit)
                {
                    success = false;
                }
            }
            return success;
        }
    }
}
