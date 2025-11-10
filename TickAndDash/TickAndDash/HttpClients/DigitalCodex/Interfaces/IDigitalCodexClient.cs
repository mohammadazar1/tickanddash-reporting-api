using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.HttpClients.GeocodingClient.DTOs;

namespace TickAndDash.HttpClients.GeoClients.Interfaces
{
    public interface IDigitalCodexClient
    {
        Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync(RegisterUserDto registerUser);
        Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token, string lang);
        //Task<DigitalCodexResponseDto<object>> GetUserBalanceAsync(string token);
        Task<DigitalCodexResponseDto<object>> CancelReservationAsync(CancelReservationRequest cancelReservation);
        Task<DigitalCodexResponseDto<object>> TransferBalanceAsync(TransferBalanceRequest transferBalance);
        Task<DigitalCodexResponseDto<object>> ReserveBalanceAsync(ReserveBalanceRequest reserveBalanceRequest);
        Task<DigitalCodexResponseDto<object>> SubscribeAysnc(SubscribeRequest subscribeRequest);
        Task<DigitalCodexResponseDto<object>> UnSubscribeAysnc(SubscribeRequest subscribeRequest);
    }
}
