using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly ICarsService _carService;

        public CarsController(ICarsService carService)
        {
            _carService = carService;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_carService.GetAllCars());
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateCarRequest createCarRequest)
        {
            return Ok(_carService.CreateCar(createCarRequest));
        }

        [HttpPut]
        public IActionResult Put([FromBody] UpdateCarRequest putCarRequest)
        {
            return Ok(_carService.UpdateCar(putCarRequest));
        }
    }
}
