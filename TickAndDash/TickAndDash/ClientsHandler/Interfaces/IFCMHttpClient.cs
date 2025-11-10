using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.ClientsHandler.Dtos;

namespace TickAndDash.ClientsHandler.Interfaces
{
    public interface IFCMHttpClient
    {
        Task<bool> PushNotificationsAsync(/*string FCMToken,*/ PushNotificationDto pushNotificationDto);
    }


}
