using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IFinancialsService
    {
        Task<IList<UserTransaction>> GetAllByFinancialsRequestAsync(FinancialsRequest financialsRequest);
        Task<IList<TotalFinancialBalance>> GetDriversFinancialsBalance();
    }
}
