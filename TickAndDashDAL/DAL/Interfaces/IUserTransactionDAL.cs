using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IUserTransactionDAL
    {
        Task<IList<UserTransaction>> GetAllByFinancialsRequestAsync(UserTransactionFilter financialsRequest);
        Task<IList<TotalFinancialBalance>> GetDriversFinancialsBalance();
    }
}
