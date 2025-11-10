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
    public class PaymentsToDriversController : ControllerBase
    {
        private readonly IPaymentsToDriversService _paymentsToDriversService;

        public PaymentsToDriversController(IPaymentsToDriversService paymentsToDriversService)
        {
            _paymentsToDriversService = paymentsToDriversService;

        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePaymentToDriversRequest createPaymentToDriversRequest)
        {
            int id = await _paymentsToDriversService.CreatePaymentToDriverAsync(createPaymentToDriversRequest);
            return Created(id.ToString(), createPaymentToDriversRequest);
        } 

        [HttpGet]
        public async Task<IActionResult> GetAllByFilter([FromQuery] GetAllByfilterPaymentsToDriversRequest request)
        {

            IList<PaymentToDriver> paymentToDrivers = await _paymentsToDriversService.GetAllByFilter(request);
            return Ok(paymentToDrivers);
        }

    }
}
