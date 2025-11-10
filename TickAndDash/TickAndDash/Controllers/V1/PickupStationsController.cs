using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Models;
using static TickAndDash.Filters.AuthorizationFilter;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]

    public class PickupStationsController : ControllerBase
    {

        private readonly ITransItinerariesService _transItinerariesService;
        private readonly IPickupStationsService _pickupStationsService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<PickupStationsController> _localizer;

        public PickupStationsController(ITransItinerariesService transItinerariesService, IPickupStationsService pickupStationsService, IAuthService authService, IMapper mapper, IStringLocalizer<PickupStationsController> localizer)
        {
            _transItinerariesService = transItinerariesService;
            _pickupStationsService = pickupStationsService;
            _authService = authService;
            _mapper = mapper;
            _localizer = localizer;
        }
        
        /// <summary>
        /// Request for getting all pickup stations relative to the itinerary in rider current site
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Data_1 No mathing data<br/>
        /// 2.Ok Success<br/>
        /// 3.param_1 BadRequest by missing paramter request or passing unvalid value<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response> 
        /// <response code = "404"> Failed </response> 
        /// <response code = "403"> Failed </response> 
        [HttpGet("")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [ProducesResponseType(typeof(GeneralResponse<List<PickupStationsResponse>>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> GetPickupStationsRelativeToItinerary
            ([FromQuery] GetPickupStationsRelativeToItineraryRequest request)
        {
            GeneralResponse<List<PickupStationsResponse>> response = new GeneralResponse<List<PickupStationsResponse>>()
            {
                Code = ""
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            string _language = Request.Headers["Content-Language"];
            int transId = await _transItinerariesService.GetTransItinerariesBySitesEndPointsAsync(request.FromSiteId, request.TowardSiteId);
            
            if (transId < 0)
            {
                response.Message = _localizer["ProcessingError"].Value; /*"خطأ في معالجة البيانات، يرجى المحاولة لاحقًا";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var mainPickupStations = await _pickupStationsService.GetSitePickupStationsByIdAndFromSiteAsync(request.FromSiteId, transId);
            if (mainPickupStations == null || !mainPickupStations.Any())
            {
                response.Message = _localizer["NoItineraryError"].Value; /* "عذرًا، لا يتوفر خطوط نقل بعد من المنطقة المختارة، يرجى اختيار منطقة أخرى";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var pickupStationsResponses = _mapper.Map<List<PickupStationsResponse>>(mainPickupStations);
            var minorPsStations = await _pickupStationsService.
                GetMinorPickupStationsInSiteWithinSpecificTrans(request.FromSiteId, transId, _language);

            if (minorPsStations != null && minorPsStations.Any())
            {
                pickupStationsResponses.AddRange(_mapper.Map<List<PickupStationsResponse>>(minorPsStations));
            }

            response.Data.AddRange(pickupStationsResponses);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            return Ok(response);
        }

        /// <summary>
        /// Request for getting all driver pickup stations that he may reserve a turn on its queue if its a main station
        /// return unique minor stations location for Drivers apps to show them in the map only 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Data_1 No mathing data<br/>
        /// 2.Ok Success<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "404"> Failed </response> 
        /// <response code = "403"> Failed </response> 
        [HttpGet("Driver")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetDriverPickupStations()
        {
            GeneralResponse<List<PickupStationsResponse>> response = new GeneralResponse<List<PickupStationsResponse>>()
            {
                Data = new List<PickupStationsResponse>(),
                Code = ""
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);

            var mainPickupStations = await _pickupStationsService.GetAllPickUpStationsByCarIdAsync(carId);
            var minorPickupStations = await _pickupStationsService.GetUniqueMinorPickupStations(carId);

            List<PickupStations> pickupStations = new List<PickupStations>();

            if (mainPickupStations != null && mainPickupStations.Any())
            {
                pickupStations.AddRange(mainPickupStations);
            }

            if (minorPickupStations != null && minorPickupStations.Any())
            {
                pickupStations.AddRange(minorPickupStations);
            }

            if (pickupStations == null || !pickupStations.Any())
            {
                response.Message = _localizer["NoPickupsError"].Value; /*"عذرًا، لاتوفر مواقف مخصصة لهذا الحساب!";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var pickupStationsResponses = _mapper.Map<List<PickupStationsResponse>>(pickupStations);

            response.Success = true;
            response.Data.AddRange(pickupStationsResponses);
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }


    }
}
