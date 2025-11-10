using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Services;
using TickAndDashDAL.Enums;

namespace TickAndDash.Filters
{
    public partial class AuthorizationFilter
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    GeneralResponse<object> response = new GeneralResponse<object>()
        //    {
        //        Data = null
        //    };
        //    var isAuth = _authService.IsUserAuthorizedAsync(role).Result;
        //    if (!isAuth.Success)
        //    {
        //        response.Success = isAuth.Success;
        //        response.Message = isAuth.Message;
        //        response.Code = isAuth.Code.ToString();
        //        context.HttpContext.Response.StatusCode = 403;
        //        context.Result = new JsonResult(response);
        //        //return StatusCode(403, response);
        //    }
        //    base.OnActionExecuting(context);
        //}

        public class TokenAuthorizationFilter : ActionFilterAttribute
        {
            private readonly IAuthService _authService;

            public TokenAuthorizationFilter(IAuthService authService)
            {
                _authService = authService;
            }



            public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                GeneralResponse<object> response = new GeneralResponse<object>()
                {
                    Data = null
                };

                var user = context.HttpContext.User;

                string role = user.FindFirstValue(ClaimTypes.Role);
                RolesEnum roleEnum = (RolesEnum)Enum.Parse(typeof(RolesEnum), role, true);


                var isAuth = await _authService.IsUserAuthorizedToLogOut(roleEnum);


                if (!isAuth.Success)
                {
                    response.Success = isAuth.Success;
                    response.Message = isAuth.Message;
                    response.Code = isAuth.Code.ToString();
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new ObjectResult(response) { StatusCode = 403 };
                    return;
                }

                await next();
            }
        }
    }
}
