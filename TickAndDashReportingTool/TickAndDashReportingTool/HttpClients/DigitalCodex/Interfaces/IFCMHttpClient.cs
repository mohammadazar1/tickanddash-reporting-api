using System.Threading.Tasks;

namespace TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces
{
    public interface IFCMHttpClient
    {
        Task<bool> PushNotificationsAsync(/*string FCMToken,*/ PushNotificationDto pushNotificationDto);
    }
}
