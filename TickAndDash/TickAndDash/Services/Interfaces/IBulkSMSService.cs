using System.Threading.Tasks;
using TickAndDash.Services.ServicesDtos;

namespace TickAndDash.Services.Interfaces
{
    public interface IBulkSMSService
    {
        Task<bool> SendSMSAsync(SMSDto sMSDto);
    }
}
