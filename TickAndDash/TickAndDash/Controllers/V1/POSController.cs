using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.HttpClients.GeoClients.Interfaces;
using TickAndDash.Services;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class POSController : ControllerBase
    {

        private readonly IPointOfSalesServices _pointOfSalesServices;
        private readonly IStringLocalizer<POSController> _localizer;
        private readonly IDigitalCodexClient _digitalCodexClient;
        private readonly IMapper _mapper;

        public POSController(IPointOfSalesServices pointOfSalesServices, IStringLocalizer<POSController> localizer, IMapper mapper, IDigitalCodexClient digitalCodexClient)
        {
            _pointOfSalesServices = pointOfSalesServices;
            _localizer = localizer;
            _mapper = mapper;
            _digitalCodexClient = digitalCodexClient;
        }



        [HttpGet("")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetAllPointOfSales([FromQuery] int siteId)
        {
            GeneralResponse<List<GetAllPointOfSales>> response = new GeneralResponse<List<GetAllPointOfSales>>()
            {
                Data = new List<GetAllPointOfSales>()
            };

            var pointOfSales = await _pointOfSalesServices.GetAllPointOfsalesAsync(siteId);

            if (pointOfSales == null || !pointOfSales.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetAllPointOfSales>>(pointOfSales);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        [HttpGet("Sites")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetAllPOSSites()
        {
            GeneralResponse<List<GetAllSitesCanVisitResponse>> response = new GeneralResponse<List<GetAllSitesCanVisitResponse>>()
            {
                Data = new List<GetAllSitesCanVisitResponse>()
            };

            var sites = await _pointOfSalesServices.GetAllPOSSitesAsync();
            if (sites == null || !sites.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetAllSitesCanVisitResponse>>(sites);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
