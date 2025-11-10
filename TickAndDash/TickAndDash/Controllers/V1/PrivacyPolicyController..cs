using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Text;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "text/html")]
    public class PrivacyPolicy : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IStringLocalizer<PrivacyPolicy> _localizer;
        public PrivacyPolicy(IWebHostEnvironment env, IStringLocalizer<PrivacyPolicy> localizer)
        {
            this.env = env;
            _localizer = localizer;
        }


        /// <summary>
        ///  request for apps privacy policy drivers and riders
        /// </summary>
        /// <returns></returns>
        /// <response code = "200"> Success </response>
        [HttpGet]
        [Authorize]
        //[Produces("text/html")]
        public IActionResult GetPrivacyPolicy()
        {
            GeneralResponse<GetPrivacyPolicyResponse> response = new GeneralResponse<GetPrivacyPolicyResponse>()
            {
                Data = new GetPrivacyPolicyResponse()
            };

            //var path = env.WebRootFileProvider.GetFileInfo("PrivacyPolicy.txt")?.PhysicalPath;
            var path = env.WebRootFileProvider.GetFileInfo("PV.html")?.PhysicalPath;

            string readContents;
            
            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                readContents = @$"{streamReader.ReadToEnd()}";
            }

            //Console.WriteLine(readContents);
            //var result = System.Text.RegularExpressions.Regex.Escape(readContents);
            //var x = Uri.UnescapeDataString(readContents);
            //HttpResponseMessage response = new HttpResponseMessage()
            //{
            //    Content = new StringContent(
            //    readContents,
            //    Encoding.UTF8,
            //    "application/json"
            //)
            //};;
            //return response;
            //return Ok(readContents);
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            response.Data.HTLM = readContents;

            return Ok(response);
            //return Content(readContents, "text/html", Encoding.UTF8);

        }
    }
}
