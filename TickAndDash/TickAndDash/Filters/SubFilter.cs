using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Services;

namespace TickAndDash.Filters
{
    public class SubFilter : ActionFilterAttribute
    {
        private readonly IAuthService _authService;
        //public RolesEnum role { get; set; }

        public SubFilter(IAuthService authService)
        {
            _authService = authService;
            //role = roles;
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null
            };

            var user = context.HttpContext.User;
            var isAuth = await _authService.IsSubscribedAsync();

            if (!isAuth.Success)
            {
                response.Success = isAuth.Success;
                response.Message = isAuth.Message;
                response.Code = isAuth.Code.ToString();
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }

            await next();
            //return base.OnActionExecutionAsync(context, next);
        }
    }
}
