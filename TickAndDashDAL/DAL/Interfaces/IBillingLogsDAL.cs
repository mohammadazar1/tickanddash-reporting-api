using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IBillingLogsDAL
    {
        Task<bool> InsertBillingLogAsync(BillingLogs billingLogs);
    }
}
