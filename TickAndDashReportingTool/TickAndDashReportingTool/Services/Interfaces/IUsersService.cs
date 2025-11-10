using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IUsersService
    {
        object Login(LoginUserRequest loginUserRequest);
        
        string Register(RegisterUserRequest registerUserRequest);
        
        Task<bool> IsMobileNumberExist(string msisdn);

        Task<Riders> GetRiderByMsisdnAsync(string msisdn);

    }
}
