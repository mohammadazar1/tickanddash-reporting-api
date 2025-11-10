using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class ItinerariesController : ControllerBase
    {
        private readonly IItinerariesService _itinerariesService;

        public ItinerariesController(IItinerariesService itinerariesService)
        {
            _itinerariesService = itinerariesService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] GetAllItinerariesRequset getAllItinerariesRequset)
        {
            IList<Transportation_Itineraries> result = _itinerariesService.GetAll(getAllItinerariesRequset);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateItineraryRequest createItinerariesRequest)
        {
            ActionResult actionResult = BadRequest();
            int itineraryId = await _itinerariesService.CreateAsync(createItinerariesRequest);

            if (itineraryId > 0)
                actionResult = Ok(new { itinerary = itineraryId });
            
            return actionResult;
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateItineraryRequest updateItineraryRequest)
        {
            bool result = await _itinerariesService.UpdateAsync(updateItineraryRequest);

            return Ok(result);
        }
    }
}
