using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.HttpClients.GeocodingClient.DTOs;

namespace TickAndDash.Services.Interfaces
{
    public interface IWalletService
    {
        Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync(string username ,string mobileNumber, string password);
        Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token);
        Task<DigitalCodexResponseDto<object>> CancelReservationAsync(CancelReservationRequest cancelReservation);
        Task<DigitalCodexResponseDto<object>> TransferBalanceAsync(TransferBalanceRequest transferBalance);
        Task<DigitalCodexResponseDto<object>> SubscribeAysnc(SubscribeRequest subscribeRequest);
        Task<DigitalCodexResponseDto<object>> UnSubscribeAysnc(SubscribeRequest subscribeRequest);

    }
}
