using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    //[Authorize]
    public class FinancialsController : ControllerBase
    {
        private readonly IFinancialsService _financialsService;

        public FinancialsController(IFinancialsService financialsService)
        {
            _financialsService = financialsService;
        }
        
        [HttpGet()]
        public async Task<ActionResult<FinancialResponse>> Get([FromQuery] FinancialsRequest financialsRequest)
        {
            IList<UserTransaction> responses = await _financialsService.GetAllByFinancialsRequestAsync(financialsRequest);
            return Ok(responses);
            
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetDriversFinancialsBalance()
        {
            IList<TotalFinancialBalance> totals = await _financialsService.GetDriversFinancialsBalance();
            return Ok(totals);
        }

    }
}
