using System.Threading.Tasks;
using TickAndDash.HttpClients.GeocodingClient.DTOs;

namespace TickAndDash.HttpClients.Interfaces
{
    public interface IGeocodingClient
    {

        Task<APIConsumingResponse<string>> GetPlaceAddressForCoordinates(decimal lat, decimal lng);

    }
}
