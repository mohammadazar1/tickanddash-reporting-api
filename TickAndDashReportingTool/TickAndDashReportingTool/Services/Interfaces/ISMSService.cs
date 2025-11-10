using System.Threading.Tasks;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ISMSService
    {

        Task<bool> SendSMSToUserAsync(string msisdn, string msg);
      
    }
}
