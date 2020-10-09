using System;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums.Validation;
using HelpMyStreetFE.Models.Validation;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class YotiController : Controller
    {
        private readonly YotiOptions _options;
        private readonly IVerificationService _verificationService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public YotiController(IOptions<YotiOptions> options, IVerificationService verificationService, IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _options = options.Value;
            _verificationService = verificationService;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Authenticate(string token, string u, bool mobile, CancellationToken cancellationToken)
        {
            var validUserId = await DecodedAndCheckedUserId(u, token != null, cancellationToken);

            if (validUserId.HasValue)
            {                                      
                return View();
            }
            else
            {
                return Redirect("/account?auth=failed");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ValidateToken(string token, string u, CancellationToken cancellationToken)
        {
            var validUserId = await DecodedAndCheckedUserId(u, token != null, cancellationToken);
            if (validUserId == null || token == null)
            {
                return Unauthorized();
            }

            try
            {
                var response = await _verificationService.ValidateUserAsync(new ValidationRequest { Token = token, UserId = validUserId.Value }, cancellationToken);
                if (response.Status == ValidationStatus.Success || response.Status == ValidationStatus.Unauthorized)
                {
                    if (response.Status == ValidationStatus.Success)
                    {
                        await _userService.CreateUserStepFiveAsync(validUserId.Value, true, cancellationToken);
                    }

                    var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
                    if (user == null)
                    {
                        // User has switched browser during mobile Yoti app flow; they're now Yoti authenticated; log them in
                        await _authService.LoginWithUserId(validUserId.Value, HttpContext, cancellationToken);
                    }
                }
                return handleValidationTokenResponse(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
        private async Task<int?> DecodedAndCheckedUserId(string encodedQueryStringUserId, bool tokenSupplied, CancellationToken cancellationToken)
        {
            try
            {
                var queryStringUserId = Base64Utils.Base64DecodeToInt(encodedQueryStringUserId);

                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

                if (tokenSupplied || user != null && user.ID == queryStringUserId)
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