using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashSharedServices.ClientsHandler.Dtos;

namespace TickAndDashSharedServices.ClientsHandler.Interfaces
{
    public interface IFCMHttpClient
    {
        Task<bool> PushNotificationsAsync(PushNotificationDto pushNotificationDto);
    }
}
