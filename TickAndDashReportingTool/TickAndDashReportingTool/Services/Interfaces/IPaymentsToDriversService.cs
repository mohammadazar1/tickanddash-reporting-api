using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IPaymentsToDriversService
    {
        Task<int> CreatePaymentToDriverAsync(CreatePaymentToDriversRequest createPaymentToDriversRequest);
        Task<IList<PaymentToDriver>> GetAllByFilter(GetAllByfilterPaymentsToDriversRequest request);
    }
}
