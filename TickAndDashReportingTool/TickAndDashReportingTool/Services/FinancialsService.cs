using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class FinancialsService : IFinancialsService
    {
        private readonly IUserTransactionDAL _userTransactionDAL;

        public FinancialsService(IUserTransactionDAL userTransactionDAL)
        {
            _userTransactionDAL = userTransactionDAL;
        }

        public async Task<IList<UserTransaction>> GetAllByFinancialsRequestAsync(FinancialsRequest financialsRequest)
        {
            IList<UserTransaction> data = await _userTransactionDAL.GetAllByFinancialsRequestAsync(new UserTransactionFilter
            {
                CarId = financialsRequest.CarId,
                Frequancy = financialsRequest.Frequancy,
                FromDate = financialsRequest.FromDate,
                ToDate = financialsRequest.ToDate,
                ItineraryId = financialsRequest.ItineraryId
            });

            return data;
        }

        public async Task<IList<TotalFinancialBalance>> GetDriversFinancialsBalance()
        {
            return await _userTransactionDAL.GetDriversFinancialsBalance();
        }
    }
}
