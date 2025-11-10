using System.Net;

namespace TickAndDash.HttpClients.GeocodingClient.DTOs
{
    public class APIConsumingResponse<T>
    {
        public bool IsDeserializeSuccess { get; set; }
        public HttpStatusCode ThirdPartyStatusCode { get; set; }
        public T ThirdPartyResponse { get; set; }

        //public override string ToString()
        //{
        //    return $"VendorResponse: {VendorResponse.ToString()}";
        //}
    }







}
