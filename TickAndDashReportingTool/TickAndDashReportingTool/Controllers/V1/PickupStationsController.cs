using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class PickupStationsController : ControllerBase
    {
        private readonly IPickupStationsService _pickupStationsService;

        public PickupStationsController(IPickupStationsService pickupStationsService)
        {
            _pickupStationsService = pickupStationsService;
        }


        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            List<PickupStations> pickups = await _pickupStationsService.GetAll();
            return Ok(pickups);
        }

        [HttpGet("{id}/main")]
        public async Task<ActionResult> GetMainByItineriesId(int id)
        {
            List<PickupStations> pickups = await _pickupStationsService.GetMainByItineriesId(id);
            return Ok(pickups);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreatePickupStationRequest createPickupStationRequest)
        {
            int pickUpId = await _pickupStationsService.Create(createPickupStationRequest);
            return Ok(pickUpId);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdatePickupRequest updatePickupRequest)
        {
            bool reuslt = await _pickupStationsService.Update(updatePickupRequest);
            return Ok(reuslt);
        }

    }
}
