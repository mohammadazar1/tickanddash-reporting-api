using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.HttpClients.GeoClients.Interfaces;
using TickAndDash.HttpClients.GeocodingClient.DTOs;
using TickAndDash.Services.Interfaces;

namespace TickAndDash.Services
{
    public class WalletService : IWalletService
    {
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly IDigitalCodexClient _digitalCodesClient;
        private ILogger<WalletService> _logger;

        public WalletService(IDigitalCodexClient digitalCodesClient, ILogger<WalletService> logger, IActionContextAccessor actionContextAccessor)
        {
            _digitalCodesClient = digitalCodesClient;
            _logger = logger;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<DigitalCodexResponseDto<object>> CancelReservationAsync(CancelReservationRequest cancelReservation)
        {

            return await _digitalCodesClient.CancelReservationAsync(cancelReservation);
        }

        public async Task<DigitalCodexResponseDto<List<DigitalCodexGetBalanceResponse>>> GetUserBalanceAsync(string token)
        {
            string language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            var response = await _digitalCodesClient.GetUserBalanceAsync(token, language);

            if (response != null)
            {
                string responseObj = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                _logger.LogInformation($"{responseObj}");
            }

            return response;
        }

        public async Task<DigitalCodexResponseDto<DigitalCodexRegisterUserResponse>> RegisterUserAsync
            (string username, string mobileNumber, string password)
        {
            RegisterUserDto registerUserDto = new RegisterUserDto()
            {
                Name = $"{username}",
                UserName = username,
                Email = username,
                MSISDN = mobileNumber,
                Password = password,
                FlowId = 3
            };

            var response = await _digitalCodesClient.RegisterUserAsync(registerUserDto);

            if (response != null)
            {
                Log.
              ForContext("RegisterRiderResponse", response, true).
              Information("Register Rider Response");
                //string responseObj = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                //_logger.LogInformation($"Register User {registerUserDto.MSISDN} Response {responseObj}");
            }

            return response;
        }

        public async Task<DigitalCodexResponseDto<object>> SubscribeAysnc(SubscribeRequest subscribeRequest)
        {
            var response = await _digitalCodesClient.SubscribeAysnc(subscribeRequest);

            if (response != null)
            {
                Log.
                    ForContext("SubscribeRiderResponse", response, true).
                    Information("Subscribe Rider Response");
                //string responseObj = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                //_logger.LogInformation($"Subscribe User {subscribeRequest.Token} Response {responseObj}");
            }

            return response;
        }

        public async Task<DigitalCodexResponseDto<object>> TransferBalanceAsync(TransferBalanceRequest transferBalance)
        {
            var response = await _digitalCodesClient.TransferBalanceAsync(transferBalance);
            if (response != null)
            {
                string responseObj = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                _logger.LogInformation($"Transfer to User {transferBalance.MobileNumber} from {transferBalance.Token} Response {responseObj}");
            }

            return response;

        }

        public async Task<DigitalCodexResponseDto<object>> UnSubscribeAysnc(SubscribeRequest request)
        {
            var response = await _digitalCodesClient.UnSubscribeAysnc(request);

            if (response != null)
            {
                string responseObj = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                _logger.LogInformation($"UnSubscribe User {request.Token} Response {responseObj}");
            }
            return response;
        }
    }
}
