using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Validation;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Validation;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class YotiController : Controller
    {
        private readonly YotiOptions _options;
        private readonly IValidationService _validationService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public YotiController(IOptions<YotiOptions> options, IValidationService validationService, IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _options = options.Value;
            _validationService = validationService;
            _authService = authService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(string token, string u)
        {
            var validUserId = DecodedAndCheckedUserId(u, token != null);

            if (validUserId != null)
            {
                User user = await _userService.GetUserAsync(int.Parse(validUserId));                        
                var viewModel = new AuthenticateViewModel {ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId };
                return View(viewModel);
            }
            else
            {
                return Redirect("/account");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ValidateToken(string token, string u, bool mobile, CancellationToken cancellationToken)
        {
            var validUserId = DecodedAndCheckedUserId(u, token != null);
            if (validUserId != null && token != null)
            {                           
                var response = await _validationService.ValidateUserAsync(new ValidationRequest { Token = token, UserId = validUserId }, cancellationToken);           
                if (response.Status == ValidationStatus.Success)
                {
                    await _userService.CreateUserStepFiveAsync(int.Parse(validUserId), true);                 
                }
                
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null) 
                {
                    // User has switched browser during mobile Yoti app flow; they're now Yoti authenticated; log them in
                    await _authService.LoginWithUserId(int.Parse(validUserId), HttpContext);                                   
                }
                if (mobile)
                {
                    string redirect = response.Status == ValidationStatus.Success ? "/account" : "/account?auth=failed";
                    return Redirect(redirect);                    
                }
                return handleValidationTokenResponse(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        public async Task<IActionResult> AuthSuccess()
        {    
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _userService.GetUserAsync(id);
            if (user.IsVerified.HasValue && user.IsVerified.Value) 
            {        
                return View();
            }
            return Redirect("/Error/500");
        }

        [AllowAnonymous]
        public async Task<IActionResult> AuthFailed(string u)
        {
            var validUserId = DecodedAndCheckedUserId(u, true);

            if (validUserId != null)
            {                                
                return View(new AuthenticateViewModel { ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId });
            }
            else
            {
                return Redirect("/Error/500");
            }
        }

        private IActionResult handleValidationTokenResponse(ValidationResponse response)
        {
            return response.Status switch
            {
                ValidationStatus.Success => Ok(response),
                ValidationStatus.Unauthorized => Unauthorized(response),
                ValidationStatus.UnexepectedError => StatusCode(500, response),
                ValidationStatus.ValidationFailed => BadRequest(response),
                _ => StatusCode(500, response)
            };
        }

        /// <summary>
        /// Base64 decodes encodedQueryStringUserId, and ensures it matches the user in session (if any)
        /// </summary>
        /// <param name="encodedQueryStringUserId"></param>
        /// <returns></returns>
        private string DecodedAndCheckedUserId(string encodedQueryStringUserId, bool tokenSupplied)
        {
            try
            {
                var queryStringUserId = Base64Helpers.Base64Decode(encodedQueryStringUserId);

                var authenticatedUserIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (tokenSupplied || authenticatedUserIdClaim != null && authenticatedUserIdClaim.Value == queryStringUserId)
                {
                    return queryStringUserId;
                }
                else
                {
                    return null;
                }
            }
            catch //(Exception ex)
            {
                return null;
            }
        }
    }
}