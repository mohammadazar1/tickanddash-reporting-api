using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickAndDashReportingTool.Services.Interfaces;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashDAL.Models;
using Microsoft.AspNetCore.Authorization;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class SitesController : ControllerBase
    {
        private ISitesService _siteService;

        public SitesController(ISitesService siteService)
        {
            _siteService = siteService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateSiteRequest createSiteRequest)
        {
            IActionResult actionResult = BadRequest();

            if (_siteService.Create(createSiteRequest))
                actionResult = Ok();

            return actionResult;
        }   

        [HttpGet]
        public IActionResult GetAll()
        {
            IActionResult actionResult = NotFound();

            IList<Site> sites = _siteService.GetAll();

            if (sites != null && sites.Count > 0)
                actionResult = Ok(sites);

            return actionResult;
        }

        [HttpGet("lookup")]
        public IActionResult GetAllLookup()
        {
            IActionResult actionResult = NotFound();

            IList<SiteLookup> sites = _siteService.GetAllLookupSites();

            if (sites != null && sites.Count > 0)
                actionResult = Ok(sites);

            return actionResult;
        }

        [HttpPut]
        public IActionResult Put([FromBody] UpdateSiteRequest updateSiteRequest) 
        {
            IActionResult actionResult = BadRequest();

            if (_siteService.Update(updateSiteRequest))
                actionResult = Ok();

            return actionResult;
        }

        [HttpDelete("id")]
        public IActionResult Delete(int id)
        {
            IActionResult actionResult = BadRequest();

            if (_siteService.Delete(id))
                actionResult = Ok();

            return actionResult;
        }

    }
}
