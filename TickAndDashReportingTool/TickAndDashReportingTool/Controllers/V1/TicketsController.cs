using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }


        [HttpGet("manual")]
        public async Task<IActionResult> GenerateManualTicketAsync([FromQuery] ManualTicketRequest manualTicketRequest)
        {
            string ticket = await _ticketService.GenerateManualTicket(manualTicketRequest);
            if (ticket == "") return BadRequest();
            
            return Ok(new { ticketCode = ticket});
        }
    }
}
