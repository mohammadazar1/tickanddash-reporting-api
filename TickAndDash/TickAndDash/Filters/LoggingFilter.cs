using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Serilog;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Enums;

namespace TickAndDash.Filters
{
    public class LoggingFilter : ActionFilterAttribute
    {

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endPoint = context.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            //var routeNAme = endPoint?.Metadata.GetMetadata<IRouteValuesAddressMetadata>()?.RouteName;
            var route = context.HttpContext.GetRouteData();
            //var endpoint = context.HttpContext.GetEndpoint();
            //string endPointName = endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
            //var xx = context.HttpContext.GetEndpoint()?.DisplayName;

            string requestBodyJson = "No Body";
            //var requestBody = context.ActionArguments?["request"];
            bool isBodyExist = context.ActionArguments.TryGetValue("request", out object requestBody);
            var requestParamters = context.HttpContext.Request.Query?.ToList();

            if (isBodyExist)
            {
                requestBodyJson = JsonConvert.SerializeObject(requestBody);
            }

            var token = context.HttpContext.Request.Headers?["Authorization"].ToString();
            int.TryParse(context.HttpContext.User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            //if (!string.IsNullOrWhiteSpace(token))
            //{
            Log.
            ForContext("token", token).
            ForContext("userId", userId).
            ForContext("requestBody", requestBodyJson).
            ForContext("requestParamters", requestParamters, true).
            ForContext("RequestId", context.HttpContext.TraceIdentifier).
            Information($"API Request Started ({route?.Values["controller"]}/{route?.Values["action"]})...");
            //}

            var result = await next();
            var res = result.Result as ObjectResult;

            Log.
            ForContext("Response", res?.Value, true).
            ForContext("RequestId", context.HttpContext.TraceIdentifier).
            ForContext("userId", userId).
            Information($"API Response ({route?.Values["controller"]}/{route?.Values["action"]})...");
        }

    }
}
