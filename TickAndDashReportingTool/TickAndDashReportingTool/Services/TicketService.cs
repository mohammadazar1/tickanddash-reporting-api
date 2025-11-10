using System;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class TicketService : ITicketService
    {
        private readonly IRidersTicketsDAL _ridersTicketsDAL;
        private readonly IUsersDAL _usersDAL;
        private readonly IRidersQueueDAL _ridersQueueDAL;
        private readonly ISystemConfigurationDAL _systemConfigurationDAL;
        private readonly ICarsTripsService _carsTripsService;
        private readonly ICarsQueuesService _carsQueuesService;

        public TicketService(IRidersTicketsDAL ridersTicketsDAL, IUsersDAL usersDAL, IRidersQueueDAL ridersQueueDAL, ISystemConfigurationDAL systemConfigurationDAL, ICarsTripsService carsTripsService, ICarsQueuesService carsQueuesService)
        {
            _ridersTicketsDAL = ridersTicketsDAL;
            _usersDAL = usersDAL;
            _ridersQueueDAL = ridersQueueDAL;
            _systemConfigurationDAL = systemConfigurationDAL;
            _carsTripsService = carsTripsService;
            _carsQueuesService = carsQueuesService;
        }

        public async Task<string> GenerateManualTicket(ManualTicketRequest manualTicketRequest)
        {
            // Check Manual Ticket user exists
            User user = await _usersDAL.GetByNameAsync("ManualTicket");
            int riderId = user.Id;

            if (user == null)
            {
                riderId = await _usersDAL.CreateManualTicketUserAsync();
                if (riderId > 0)
                    return "";
            }

            // Reserve turn
            string reserveTimeLimit = await _systemConfigurationDAL.
                GetSettingValueByKeyAsync(SettingKeyEnum.ReserveTimeLimit);

            int riderQId = await _ridersQueueDAL.CreateManualTicket(new RidersQueue
            {
                PickupStationId = manualTicketRequest.PickupStationId,
                RiderId = user.Id,
                ReservationDate = DateTime.Now.AddMinutes(int.Parse(reserveTimeLimit)),
                CountOfSeats = 1,
                IsInQueue = true,
                RidersQStatusLookupId = (int)RidersQStatusLookupEnum.Ticket
            });

            // Generate ticket
            // Commented
            //bool ticketCreated = await _ridersTicketsDAL.AddRidersTicketAsync(new RidersTickets
            //{
            //    RiderQId = riderQId,
            //    Ticket = "testTickt"
            //});

            return riderQId.ToString();
        }
    }
}
