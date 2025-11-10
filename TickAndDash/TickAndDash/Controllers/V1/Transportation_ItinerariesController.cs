using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services.Interfaces;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class Transportation_ItinerariesController : ControllerBase
    {
        private readonly ITransItinerariesService _transItinerariesService;
        private readonly IStringLocalizer<Transportation_ItinerariesController> _localizer;
        private readonly IMapper _mapper;

        public Transportation_ItinerariesController(ITransItinerariesService transItinerariesService, IStringLocalizer<Transportation_ItinerariesController> localizer, IMapper mapper)
        {
            _transItinerariesService = transItinerariesService;
            _localizer = localizer;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize()]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetGetItineraries()
        {
            GeneralResponse<List<GetItinerariesRequest>> response = new GeneralResponse<List<GetItinerariesRequest>>()
            {
                Data = new List<GetItinerariesRequest>(),
                Code = ""
            };

            var transportation_Itineraries = await _transItinerariesService.GetTransportation_ItinerariesAsync();
            if (transportation_Itineraries == null || !transportation_Itineraries.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetItinerariesRequest>>(transportation_Itineraries);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
