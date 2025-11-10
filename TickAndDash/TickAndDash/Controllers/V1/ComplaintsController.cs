using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class ComplaintsController : ControllerBase
    {
        private readonly IStringLocalizer<ComplaintsController> _localizer;
        private readonly IComplaintsServices _complaintsServices;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public ComplaintsController(IStringLocalizer<ComplaintsController> localizer,
            IComplaintsServices complaintsServices, IAuthService authService, IMapper mapper)
        {
            _localizer = localizer;
            _complaintsServices = complaintsServices;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost()]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        public async Task<IActionResult> AddComplaints(AddComplaintsRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            var complaint = _mapper.Map<Complaint>(request);

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            complaint.UserId = userId;

            bool isInserted = await _complaintsServices.InsertComplaintAsync(complaint);

            if (!isInserted)
            {
                return UnprocessableEntity(response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        [HttpPost("ComplaintsReply")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        public async Task<IActionResult> AddComplaintsReply(AddComplaintsReplyRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            var complaint = await _complaintsServices.GetComplaintByIdAsync(request.ComplaintId);

            if (complaint == null)
            {
                response.Message = _localizer["NoComplaint"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }
            else if (complaint.UserId != userId)
            {
                response.Message = _localizer["NoComplaint"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return StatusCode(422, response);
            }

            var complaintStory = _mapper.Map<ComplaintsTickets>(request);
            complaintStory.UserId = userId;

            bool isAdded = await _complaintsServices.AddComplaintReplyAsync(complaintStory);

            if (!isAdded)
            {
                return UnprocessableEntity(response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        [HttpGet()]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        public async Task<IActionResult> GetRiderComplaints()
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            //var isAuth = _authService.IsUserAuthorized(RolesEnum.Rider);

            //if (!isAuth.Success)
            //{
            //    response.Success = isAuth.Success;
            //    response.Message = isAuth.Message;
            //    response.Code = isAuth.Code.ToString();
            //    return StatusCode(403, response);
            //}

            var complaints = await _complaintsServices.GetComplaintsByUserId(userId);

            if (complaints == null)
            {
                return NotFound(response);
            }

            var complaintsTickets = await _complaintsServices.GetComplaintTickets(complaints);

            return Ok(complaintsTickets);
        }


        [HttpGet("Types")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetComplaintsType()
        {
            GeneralResponse<List<GetAllComplaintsType>> response = new GeneralResponse<List<GetAllComplaintsType>>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            string _language = Request.Headers["Content-Language"];

            var complaintTypes = await _complaintsServices.GetAllComplaintsTypeAsync(_language);

            if (complaintTypes == null || !complaintTypes.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetAllComplaintsType>>(complaintTypes);

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        [HttpGet("SubTypes/{id:int}")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetComplaintsSubType(int id)
        {
            GeneralResponse<List<GetAllComplaintsSubType>> response = new GeneralResponse<List<GetAllComplaintsSubType>>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            string _language = Request.Headers["Content-Language"];
            var complaintSubTypes = await _complaintsServices.GetAllComplaintsTypeSubTypeAsync(id, _language);

            if (complaintSubTypes == null || !complaintSubTypes.Any())
            {
                response.Message = _localizer["NoDataMsg"].Value;
                return NotFound(response);
            }

            response.Data = _mapper.Map<List<GetAllComplaintsSubType>>(complaintSubTypes);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
