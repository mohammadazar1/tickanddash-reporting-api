using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashDAL.Models.CustomModels;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Services;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    public class PosController : ControllerBase
    {
        private readonly IPosService _posService;
        private readonly IDigitalCodexClient _digitalCodexClient;
        private readonly IUserTransactionsService _userTransactionsService;
        private readonly IUsersService _usersService;
        private readonly ISMSService _sMSService;

        public PosController(IPosService posService, IDigitalCodexClient digitalCodexClient, IUserTransactionsService userTransactionsService,
            IUsersService usersService, ISMSService sMSService)
        {
            _posService = posService;
            _digitalCodexClient = digitalCodexClient;
            _userTransactionsService = userTransactionsService;
            _usersService = usersService;
            _sMSService = sMSService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IList<PointOfSales> poss = await _posService.GetAll();

            foreach (var pos in poss)
            {
                var balamceResponse = await _digitalCodexClient.GetUserBalanceAsync(pos.DCToken);
                pos.DCToken = "";
                if (balamceResponse.Success)
                {
                    pos.Balance = balamceResponse.Data.Find(x => x.CurrencyCode == "ILS")?.BalanceCanBeUsed ?? 0;
                }
            }


            return Ok(poss);

        }

        [HttpPost]
        public async Task<IActionResult> Post(PointOfSales pointOfSales)
        {
            bool posId = await _posService.CreateAsync(pointOfSales);
            return Ok(posId);
        }

        [HttpPut]
        public async Task<IActionResult> Put(PointOfSales pointOfSales)
        {
            bool result = await _posService.UpdateAsync(pointOfSales);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _posService.DeleteAsync(id);
            return Ok(result);
        }


        [Authorize(Roles = "POS")]
        [HttpPost("Transfer")]
        public async Task<IActionResult> POSTransferBalance(CreateTransferBalanceRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>
            {
                MessageAr = "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا",
                MessageEn = "Sorry! something went wrong, please try again later",
                Data = null
            };
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.Name));
            var pos = await _posService.GetPointOfSaleByIdAsync(userId);

            if (pos == null)
            {
                return UnprocessableEntity(response);
            }
            if (request.MSISDN?.Length < 9)
            {
                response.MessageAr = "عذرًا، رقم الموبايل غير صحيح";
                response.MessageEn = "Sorry! wrong mobile number inserted";
                return UnprocessableEntity(response);
            }
            else
            {
                request.MSISDN = $"972{request.MSISDN.Substring(request.MSISDN.Length - 9)}";
            }
            if (request.Amount < 1)
            {
                response.MessageAr = "عذرًا، يرجى إدخال الكمية المراد تحويلها";
                response.MessageEn = "Sorry! please insert the amount that you want to transfer";
                return UnprocessableEntity(response);
            }
            var rider = await _usersService.GetRiderByMsisdnAsync(request.MSISDN);
            if (rider == null)
            {
                response.MessageAr = "عذرًا، رقم الموبايل المراد التحويل اليه لا يمتلك حساب";
                response.MessageEn = "Sorry!, Mobile number does not have an  account";
                return UnprocessableEntity(response);
            }

            var transferResponse = await _digitalCodexClient.TransferBalanceAsync(new TransferBalanceRequest
            {
                MobileNumber = request.MSISDN,
                TransferBalance = request.Amount,
                TransferCurrency = "ILS",
                Username = pos.Username,
                Token = pos.DCToken
            });
            response.MessageAr = transferResponse.MessageAr;
            response.MessageEn = transferResponse.MessageEn;

            if (transferResponse != null && transferResponse.Success)
            {

                await _userTransactionsService.InsertUserTransactions(new UserTransactions
                {
                    Amount = request.Amount,
                    CreationDate = DateTime.Now,
                    FromUserId = pos.UserId,
                    ToUserId = rider.UserId,
                    Type = "POSTransfer",
                    UserTransactionTypeId = 4
                });
                response.Success = true;
                bool success = await _sMSService.SendSMSToUserAsync(rider.MobileNumber, $"تم تحويل مبلغ {request.Amount} إلى حسابك في Tick&Dash");
                return Ok(response);
            }

            return UnprocessableEntity(response);
        }


        [Authorize(Roles = "POS")]
        [HttpGet("Balance")]
        public async Task<IActionResult> GetPOSBalance()
        {
            GeneralResponse<object> response = new GeneralResponse<object>
            {
                MessageAr = "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا",
                MessageEn = "Sorry! something went wrong, please try again later",
                Data = null
            };

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.Name));
            var pos = await _posService.GetPointOfSaleByIdAsync(userId);
            var balamceResponse = await _digitalCodexClient.GetUserBalanceAsync(pos.DCToken);

            if (balamceResponse == null)
            {
                return UnprocessableEntity(response);
            }

            if (balamceResponse.Success)
            {
                response.MessageAr = "تم معالجة طلبك بنجاح!";
                response.MessageEn = "Success";
                response.Success = true;
                response.Data = balamceResponse.Data.Find(x => x.CurrencyCode == "ILS")?.BalanceCanBeUsed ?? 0;
                return Ok(response);
            }
            else
            {

                response.MessageAr = balamceResponse?.MessageAr ?? response.MessageAr;
                response.MessageEn = balamceResponse?.MessageEn ?? response.MessageAr;

                return UnprocessableEntity(response);
            }

        }

        [Authorize(Roles = "POS, Admin")]
        [HttpGet("TopUpTransactions")]
        public async Task<IActionResult> GetTopUpTransactions([FromQuery] GetTopUpTransactions request)
        {
            GeneralResponse<List<POSTransactions>> response = new GeneralResponse<List<POSTransactions>>
            {
                MessageAr = "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا",
                MessageEn = "Sorry! something went wrong, please try again later",
                Data = new List<POSTransactions>()
            };

            string role = User.FindFirstValue(ClaimTypes.Role);
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.Name));


            if (request.MobileNumber?.Length < 9)
            {
                response.MessageAr = "عذرًا، رقم الموبايل غير صحيح";
                response.MessageEn = "Sorry! wrong mobile number inserted";
                return UnprocessableEntity(response);
            }

            if (role == "POS")
            {
                response.Success = true;

                var pOSTransactions = await _userTransactionsService.GetPOSTransactions(userId, request.MobileNumber, request.From, request.To, isPos: true);

                if (pOSTransactions != null)
                {
                    response.Data = pOSTransactions;
                }
            }
            else
            {
                int posId = request.POSId > 0 ? request.POSId : 0;
                var pOSTransactions = await _userTransactionsService.GetPOSTransactions(posId, request.MobileNumber, request.From, request.To, isPos: false);
                if (pOSTransactions != null)
                {
                    response.Data = pOSTransactions;
                }
            }


            response.MessageAr = "تم معالجة طلبك بنجاح!";
            response.MessageEn = "Success";
            return Ok(response);

            //_digitalCodexClient.GetUserBalanceAsync
            //return UnprocessableEntity(response);
        }
    }


}
