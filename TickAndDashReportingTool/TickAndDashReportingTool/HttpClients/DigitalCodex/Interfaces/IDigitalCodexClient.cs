using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces
{
    public interface IDigitalCodexClient
    {
        Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync(RegisterUserDto registerUser);
        Task<DigitalCodexResponseDto<object>> TransferBalanceAsync(TransferBalanceRequest transferBalanceRequest);
        Task<DigitalCodexResponseDto<object>> ConsumeFromDriverAsync(string mobileNumber, double paymentAmount, string currancyType);
        Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token);
    }
}
