using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]

    public class SitesController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ISitesServices _sitesServices;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SitesController> _localizer;


        public SitesController(ILocationService locationService, ISitesServices citiesServices, IMapper mapper, IAuthService authService, IStringLocalizer<SitesController> localizer)
        {
            _locationService = locationService;
            _sitesServices = citiesServices;
            _mapper = mapper;
            _localizer = localizer;
        }


        /// <summary>
        /// Request for getting all sites registered in our end so rider could pick his startup locations
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Ok Success<br/>
        /// 2.data_1 No mathing data to process your request<br/>
        /// 3.Auth_1 token is not valid<br/>
        /// 4.Auth_2 rider session is expired due to opening another one<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "404"> No data found </response>
        /// <response code = "403"> Failed </response>
        [HttpGet("")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [ProducesResponseType(typeof(GeneralResponse<List<GetActiveSitesResponse>>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetAllActiveSites(/*[FromQuery] CitiesToGoRequest request*/)
        {
            GeneralResponse<List<GetActiveSitesResponse>> response = new GeneralResponse<List<GetActiveSitesResponse>>
            {
                Data = new List<GetActiveSitesResponse>()
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            var sites = await _locationService.GetAllSitesThasHasMainPickupStationAsync(true, userId);

            if (sites == null || !sites.Any())
            {
                response.Success = false;
                response.Message = _localizer["NoActiveSites"];
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var sitesToGo = _mapper.Map<List<GetActiveSitesResponse>>(sites);

            //response.Data = sitesToGo;
            response.Data.AddRange(sitesToGo);
            response.Success = true;
            response.Message = _localizer["SuccessResponse"];
            response.Code = Generalcodes.Ok.ToString();
            return Ok(response);
        }

        /// <summary>
        ///  request for getting all sites which rider can go to based on his current location
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Ok Success<br/>
        /// 2.data_1 No mathing data to process your request<br/>
        /// 3.Auth_1 token is not valid<br/>
        /// 4.Auth_2 rider session is expired due to opening another one<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "401"> Failed </response>
        /// <response code = "404"> No data found </response>
        [HttpGet("Visit/{siteId:int}")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<List<GetAllSitesCanVisitResponse>>), 200)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> FindSitesToVisitBasedOnSite([FromRoute] int siteId)
        {
            GeneralResponse<List<GetAllSitesCanVisitResponse>> response = new GeneralResponse<List<GetAllSitesCanVisitResponse>>
            {
                Data = null
            };

            var sites = await _locationService.GetSitesToVisitGoFromSpecificSiteAsync(siteId);

            if (sites == null || !sites.Any())
            {
                response.Success = false;
                response.Message = _localizer["NoDestenation"];
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var mapedSites = _mapper.Map<List<GetAllSitesCanVisitResponse>>(sites);

            response.Data = new List<GetAllSitesCanVisitResponse>();
            response.Data.AddRange(mapedSites);
            response.Success = true;
            response.Message = _localizer["SuccessReponse"];
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}



//bool isRequestValid = ValidateLatAndLong(request.Lat, request.Long);
//if (!isRequestValid)
//{
//    response.Code = ValidationCodes.LatLng_1.ToString();
//    response.Message = "عذرًا، خطأ في تحديد الاحداثيات";
//    return UnprocessableEntity(response);
//}