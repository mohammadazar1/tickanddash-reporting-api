using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbacksController : ControllerBase
    {

        private readonly ICallbackServices _callbackServices;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _accessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUserTransactionsService _userTransactionsService;
        private readonly IUsersService _usersService;
        public CallbacksController(ICallbackServices callbackServices, IMapper mapper, IHttpContextAccessor accessor, IWebHostEnvironment hostingEnvironment
            , IUserTransactionsService userTransactionsService, IUsersService usersService
            )
        {
            _callbackServices = callbackServices;
            _mapper = mapper;
            _accessor = accessor;
            _hostingEnvironment = hostingEnvironment;
            _userTransactionsService = userTransactionsService;
            _usersService = usersService;
        }


        [HttpPost]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> CallbacksReceiver([FromForm] AddCallbackRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password))
            {
                return UnprocessableEntity("invalid credential");
            }

            if (request.Username != "tickAndDash@ut"
                && request.Password != "C$DpG8MTmVnU"
                )
            {
                return Unauthorized();
            }

            string ipAddress = _accessor.HttpContext.Connection?.RemoteIpAddress?.ToString();

            if (_hostingEnvironment.EnvironmentName != Environments.Development)
            {
                //if (ipAddress != "")
                //{
                //    return StatusCode(403, "You are not allowed to use this service");
                //}
            }


            request.Status = request.Status.ToLower();
            if (request.Status.Contains("renewed") || request.Status.Contains("charged"))
            {
                var rider = await _usersService.GetRiderByMobileNumberAsync(request.MSISDN);
                if (rider != null)
                {
                    await _userTransactionsService.AddUserTransactionAsync(new UserTransactions
                    {
                        CreationDate = DateTime.Now,
                        FromUserId = rider.UserId,
                        ToUserId = 187,
                        Type = UserTransactionsTypesEnum.Subscription.ToString(),
                        Amount = 4,
                        UserTransactionTypeId = (int)UserTransactionsTypesEnum.Subscription
                    });
                }
            }

            var callback = _mapper.Map<Callback>(request);
            bool isInserted = await _callbackServices.InsertCallbackAsync(callback);

            if (!isInserted)
            {
                return UnprocessableEntity();
            }

            return Ok();
        }
    }
}
