using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IPaymentsToDriversDAL
    {
        Task<int> CreateAsync(PaymentToDriver paymentToDriver);
        Task<IList<PaymentToDriver>> GetAllByFilterAsync(DateTime from, DateTime to, int driverId);
    }
}
