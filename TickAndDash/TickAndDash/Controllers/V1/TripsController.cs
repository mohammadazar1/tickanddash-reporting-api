using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services.Interfaces;
using static TickAndDashDAL.DAL.TripsRidersDAL;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class TripsController : ControllerBase
    {

        private readonly ITripsService _tripsService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<TripsController> _localizer;
        private readonly IRidersQueueService _ridersQueueService;
        public TripsController(ITripsService tripsService, IMapper mapper, IStringLocalizer<TripsController> localizer,
            IRidersQueueService ridersQueueService)
        {
            _tripsService = tripsService;
            _mapper = mapper;
            _localizer = localizer;
            _ridersQueueService = ridersQueueService;
        }

        // android only hit it when login, or after leaving the app
        // and open it agian !!

        [HttpGet()]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetRiderTrips()
        {
            GeneralResponse<GetRiderTripsAndCancellationCountResponse> response = new GeneralResponse<GetRiderTripsAndCancellationCountResponse>()
            {
                Data = null
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int riderId);
            
            var ridersTrip = await _tripsService.GetRidersTripAsync(riderId);
            
            if(ridersTrip == null)
            {
                response.Success = false;
                response.Message = _localizer["NoTripsYet"].Value; /*"لم تكمل اي رحلة بعد";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = new GetRiderTripsAndCancellationCountResponse();
            int cancellationCount = await _ridersQueueService.GetRaiderCancellationCountAsync(riderId);
            
            response.Data.Trips = ridersTrip;
            response.Data.CancellationCount = cancellationCount;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

    }
}
