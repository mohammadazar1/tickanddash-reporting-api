using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceListController : ControllerBase
    {
        private readonly ITransItinerariesService _transItinerariesService;
        private readonly IStringLocalizer<PriceListController> _localizer;
        private readonly IMapper _mapper;

        public PriceListController(ITransItinerariesService transItinerariesService, IStringLocalizer<PriceListController> localizer, IMapper mapper)
        {
            _transItinerariesService = transItinerariesService;
            _localizer = localizer;
            _mapper = mapper;
        }

        [HttpGet()]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetItineraryPrices([FromQuery] int itineraryId)
        {
            GeneralResponse<List<GetItineraryPrices>> response = new GeneralResponse<List<GetItineraryPrices>>()
            {
                Data = new List<GetItineraryPrices>(),
                Code = ""
            };


            var data = await _transItinerariesService.GetTransportationsIternaryPrices(itineraryId);
            if (data == null || !data.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetItineraryPrices>>(data);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
