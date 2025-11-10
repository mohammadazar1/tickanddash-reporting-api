using System.Threading.Tasks;
using TickAndDash.Services.ServicesDtos;
using TickAndDashDAL.Enums;

namespace TickAndDash.Services
{
    public interface IAuthService
    {
        Task<IsAtuhDTO> IsUserAuthorizedAsync(RolesEnum rolesEnum);

        Task<IsAtuhDTO> IsUserAuthorizedToLogOut(RolesEnum rolesEnum);

        IsAtuhDTO IsTokenValid();
        Task<IsAtuhDTO> IsSubscribedAsync();
    }
}
