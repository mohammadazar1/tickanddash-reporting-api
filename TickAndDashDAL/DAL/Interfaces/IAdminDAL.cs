using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IAdminDAL
    {
        bool Insert(Admin admin);
        Admin GetByUserId(int userId);
        Admin GetByUserName(string username);

    }
}
