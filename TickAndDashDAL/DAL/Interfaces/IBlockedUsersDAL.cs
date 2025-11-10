using System.Threading.Tasks;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IBlockedUsersDAL
    {
         Task<bool> IsUserBlockedAsync(int userId);

    }
}
