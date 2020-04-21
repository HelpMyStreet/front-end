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
        public IActionResult Authenticate(string token, string u)
        {
            var validUserId = ValidUserId(u, token);

            if (validUserId != null)
            {
                var viewModel = new AuthenticateViewModel {ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId };
                return View(viewModel);
            }
            else
            {
                return Redirect("Registration/StepFive");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ValidateToken(string token, string u, CancellationToken cancellationToken)
        {
            var validUserId = ValidUserId(u, token);

            if (validUserId != null && token != null)
            {
                var response = await _validationService.ValidateUserAsync(new ValidationRequest { Token = token, UserId = validUserId }, cancellationToken);
                if (response.Status == ValidationStatus.Success)
                {
                    await _userService.CreateUserStepFiveAsync(int.Parse(validUserId), true);

                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value == null)
                    {
                        // User has switched browser during mobile Yoti app flow; they're now Yoti authenticated; log them in
                        await _authService.LoginWithUserId(int.Parse(validUserId), HttpContext);
                    }
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

        public async Task<IActionResult> AuthFailed()
        {
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _userService.GetUserAsync(id);
            // stops people navigating to the url when theyre not supposed to
            if (user.RegistrationHistory.Count == 4) {
                return View(new AuthenticateViewModel { ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId });
            }
            return Redirect("/Error/500");
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

        private string ValidUserId(string encodedQueryStringUserId, string token)
        {
            try
            {
                var queryStringUserId = Base64Helpers.Base64Decode(encodedQueryStringUserId);

                var authenticatedUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (authenticatedUserId != null)
                {
                    if (authenticatedUserId == queryStringUserId)
                    {
                        // User in session, and query string as expected.  First visit to this page, desktop journey, or mobile journey that started in default browser.
                        return queryStringUserId;
                    }
                }
                else
                {
                    if (token != null)
                    {
                        // No user in session, but we've got a user ID from the query string and a Yoti token.  Mobile journey that started in non-default browser.
                        return queryStringUserId;
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return null;
        }
    }
}