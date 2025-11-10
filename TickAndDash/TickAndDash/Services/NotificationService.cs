using System.Threading.Tasks;
using TickAndDash.ClientsHandler.Dtos;
using TickAndDash.ClientsHandler.Interfaces;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Enums;

namespace TickAndDash.Services
{
    public class NotificationService : INotificationService
    {
        //private readonly 

        private readonly IFCMHttpClient _fCMHttpClient;

        public NotificationService(IFCMHttpClient fCMHttpClient)
        {
            _fCMHttpClient = fCMHttpClient;
        }

        public async Task<bool> SendNotificationAsync(string to, string body,
            string title, string click_action, string category,
            string mobileType, RolesEnum role, int RiderId = 0, decimal Amount = 0)
        {
            PushNotificationDto pushNotificationDto = new PushNotificationDto();
            if (role == RolesEnum.Driver && mobileType?.ToLower() == "android")
            {
                pushNotificationDto = new PushNotificationDto
                {
                    data = new Data()
                    {
                        title = title,
                        body = body,
                        category = category
                    },
                    to = to
                };
            }
            else
            {


                pushNotificationDto = new PushNotificationDto
                {
                    data = new Data()
                    {
                        title = title,
                        body = body,
                        category = category,
                        sound = "default"
                    },
                    notification = new Notification
                    {
                        title = title,
                        body = body,
                        category = category,
                        sound = "default"
                    },
                    to = to
                };

                if (!string.IsNullOrWhiteSpace(click_action))
                {
                    pushNotificationDto.data.click_action = click_action;
                    pushNotificationDto.notification.click_action = click_action;
                }

            }

            if (RiderId > 0)
            {
                pushNotificationDto.data.RiderId = RiderId.ToString();
                pushNotificationDto.data.Amount = Amount.ToString();
            }

            return await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);
        }


    }
}