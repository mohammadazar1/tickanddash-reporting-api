using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TickAndDash.Enums;
using TickAndDash.Services.Interfaces;
using TickAndDash.Services.ServicesDtos;
using TickAndDashDAL.Enums;

namespace TickAndDash.Services
{
    public class AuthService : IAuthService
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUsersService _usersService;
        private readonly ICarService _carServices;
        private readonly IBlacklistedService _blacklistedService;
        private readonly IStringLocalizer<AuthService> _localizer;

        public AuthService(IActionContextAccessor actionContextAccessor, IUsersService usersService, ICarService carServices,
            IBlacklistedService blacklistedService, IStringLocalizer<AuthService> stringLocalizer)
        {
            _actionContextAccessor = actionContextAccessor;
            _usersService = usersService;
            _carServices = carServices;
            _blacklistedService = blacklistedService;
            _localizer = stringLocalizer;
        }

        public IsAtuhDTO IsTokenValid()
        {
            IsAtuhDTO isAtuhDTO = new IsAtuhDTO()
            {
                Success = true,
                Code = Generalcodes.Ok.ToString()
            };

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var user = _actionContextAccessor.ActionContext.HttpContext.User;

            if (!IsTokenValied(request, user))
            {
                isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                isAtuhDTO.Message = _localizer["TokenExpired"].Value; ;/* "انتهت صلاحية طلبك، يرجى اعادة تسجيل الدخول";*/
                return isAtuhDTO;
            }

            return isAtuhDTO;
        }

        public async Task<IsAtuhDTO> IsUserAuthorizedToLogOut(RolesEnum rolesEnum)
        {
            IsAtuhDTO isAtuhDTO = new IsAtuhDTO()
            {
                Success = true,
                Code = Generalcodes.Ok.ToString()
            };

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var user = _actionContextAccessor.ActionContext.HttpContext.User;


            // no need now
            if (rolesEnum == RolesEnum.Rider)
            {
                var isActive = await IsRiderSessionActiveAsync(user, request);
                if (!isActive)
                {
                    isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                    isAtuhDTO.Success = false;
                    isAtuhDTO.Message = _localizer["AlreadyLogIn"].Value; /*"تم تسجيل الدخول من جهاز اخر في وقت سابق، يرجى اعادة تسجيل الدخول";*/
                    return isAtuhDTO;
                }
            }

            if (!IsTokenValied(request, user))
            {
                isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                isAtuhDTO.Message = _localizer["TokenExpired"].Value; /*"انتهت صلاحية طلبك، يرجى اعادة تسجيل الدخول";*/
                isAtuhDTO.Success = false;
                return isAtuhDTO;
            }

            return isAtuhDTO;
        }
        public async Task<IsAtuhDTO> IsUserAuthorizedAsync(RolesEnum rolesEnum)
        {
            IsAtuhDTO isAtuhDTO = new IsAtuhDTO()
            {
                Success = true,
                Code = Generalcodes.Ok.ToString()
            };

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var user = _actionContextAccessor.ActionContext.HttpContext.User;
            // no need now
            //if (rolesEnum == RolesEnum.Rider)
            //{
            //    var isActive = await IsRiderSessionActiveAsync(user, request);
            //    if (!isActive)
            //    {
            //        isAtuhDTO.Success = false;
            //        isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
            //        isAtuhDTO.Message = _localizer["AlreadyLogIn"].Value; /*"تم تسجيل الدخول من جهاز اخر في وقت سابق، يرجى اعادة تسجيل الدخول";*/
            //        return isAtuhDTO;
            //    }
            //}

            if (!IsTokenValied(request, user))
            {
                isAtuhDTO.Success = false;
                isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                isAtuhDTO.Message = _localizer["TokenExpired"].Value; /*"انتهت صلاحية طلبك، يرجى اعادة تسجيل الدخول";*/
                return isAtuhDTO;
            }
            if (rolesEnum == RolesEnum.Driver)
            {
                int driverId = await GetTheActiveDriverInHisCab(user);
                if (driverId == 0)
                {
                    isAtuhDTO.Success = false;
                    isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                    isAtuhDTO.Message = _localizer["NoDriver"].Value; /* "لا يتواجد سائق فعال على السيارة حاليًا";*/
                    return isAtuhDTO;
                }
                    
                int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
                    
                if (driverId != userId)
                {
                    isAtuhDTO.Success = false;
                    isAtuhDTO.Code = Generalcodes.Auth_2.ToString();
                    isAtuhDTO.Message = _localizer["AnotherDriverActive"].Value; /* "تم تسجيل الدخول من جهاز اخر، يرجى اعادة تسجيل الدخول";*/
                    return isAtuhDTO;
                };
            }

            return isAtuhDTO;
        }
        private bool IsTokenValied(HttpRequest request, ClaimsPrincipal user)
        {
            bool success = true;
            int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            string headerToken = request.Headers["authorization"];
            if (!string.IsNullOrWhiteSpace(headerToken))
            {
                headerToken = Regex.Split(headerToken, "Bearer ", RegexOptions.IgnoreCase)?[1];
            }
            bool isExpired = _blacklistedService.IsTokenExpired(headerToken, userId);
            if (isExpired)
            {
                success = false;
            }
            return success;

            //bool success = true;
            //bool isTokenInCookies = request.Cookies.TryGetValue("token", out string cookiesToken);
            //if (!isTokenInCookies)
            //{
            //    success = false;
            //    return success;
            //}
            //string headerToken = request.Headers["authorization"];
            //if (!string.IsNullOrWhiteSpace(headerToken))
            //{
            //    headerToken = Regex.Split(headerToken,"Bearer ", RegexOptions.IgnoreCase)?[1];
            //}
            //if (headerToken != cookiesToken)
            //{
            //    success = false;
            //}
            //return success;
        }
        private async Task<bool> IsRiderSessionActiveAsync(ClaimsPrincipal user, HttpRequest request)
        {
            string headerToken = request.Headers["authorization"];
            string mobileNumber = user.FindFirstValue(ClaimsEnum.MobileNumber.ToString());

            if (mobileNumber is null)
            {
                return false;
            }

            var rider = await _usersService.GetRiderByMobileNumberAsync(mobileNumber);

            if (!string.IsNullOrWhiteSpace(headerToken))
            {
                headerToken = Regex.Split(headerToken, "Bearer ", RegexOptions.IgnoreCase)?[1];
            }

            if (rider.User.Token != headerToken)
            {
                return false;
            }

            return true;
        }
        private async Task<bool> IsDriverActiveInHisCab(ClaimsPrincipal user)
        {
            bool success = true;

            int.TryParse(user.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            if (carId == 0 || userId == 0)
            {
                success = false;
            }

            int driverId = await _carServices.GetCarActiveDriverIdByCarIdAsync(carId);


            if (driverId != userId)
            {
                success = false;
            }

            return success;
        }
        private async Task<int> GetTheActiveDriverInHisCab(ClaimsPrincipal user)
        {

            int.TryParse(user.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);

            return await _carServices.GetCarActiveDriverIdByCarIdAsync(carId);
        }
        public async Task<IsAtuhDTO> IsSubscribedAsync()
        {
            IsAtuhDTO isAtuhDTO = new IsAtuhDTO()
            {
                Success = false,
                Code = ""
            };

            var user = _actionContextAccessor.ActionContext.HttpContext.User;
            string mobileNumber = user.FindFirstValue(ClaimsEnum.MobileNumber.ToString());

            if (mobileNumber is null)
            {
                return isAtuhDTO;
            }

            var rider = await _usersService.GetRiderByMobileNumberAsync(mobileNumber);
            if (rider.ActiveSubscriptionPeriod > DateTime.Now)
            {
                isAtuhDTO.Success = true;
                return isAtuhDTO;
            }

            isAtuhDTO.Success = false;
            isAtuhDTO.Code = Generalcodes.Sub_1.ToString();
            isAtuhDTO.Message = _localizer["NotSubscribed"].Value;

            return isAtuhDTO;
        }
    }
}
