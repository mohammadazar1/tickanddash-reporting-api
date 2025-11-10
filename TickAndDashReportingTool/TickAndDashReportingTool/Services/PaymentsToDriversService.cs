using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class PaymentsToDriversService : IPaymentsToDriversService
    {
        private readonly IPaymentsToDriversDAL  _paymentsToDriversDAL;
        private readonly IDigitalCodexClient    _digitalCodexClient;
        private readonly IDriversDAL            _driverDAL;


        public PaymentsToDriversService(IPaymentsToDriversDAL   paymentsToDriversDAL, 
                                        IDigitalCodexClient     digitalCodexClient,
                                        IDriversDAL driversDAL)
        {
            _paymentsToDriversDAL   = paymentsToDriversDAL;
            _digitalCodexClient     = digitalCodexClient;
            _driverDAL              = driversDAL;
        }

        public async Task<int> CreatePaymentToDriverAsync(CreatePaymentToDriversRequest request)
        {
            var user = await _driverDAL.GetDriverByUserIdAsync(request.DriverId);

            var result = await _digitalCodexClient.ConsumeFromDriverAsync(user.MobileNumber, 
                                                                          request.PaymentAmount, 
                                                                          currancyType: "ILS");
            if (!result.Success)
                throw new HttpStatusException(System.Net.HttpStatusCode.Conflict, 
                                              result.Code + " - " + result.MessageAr);

            int id = await _paymentsToDriversDAL.CreateAsync(new PaymentToDriver
                                                            {
                                                                DriverId = request.DriverId,
                                                                PaymentAmount = request.PaymentAmount,
                                                            });

            return id;
        }

        public async Task<IList<PaymentToDriver>> GetAllByFilter(GetAllByfilterPaymentsToDriversRequest request)
        {
            return await _paymentsToDriversDAL.GetAllByFilterAsync(request.From,
                                                                   request.To,
                                                                   request.DriverId);
        }
    }
}