﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Validation;
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

        public YotiController(IOptions<YotiOptions> options, IValidationService validationService, IUserService userService)
        {
            _userService = userService;
            _options = options.Value;
            _validationService = validationService;
        }

        public IActionResult Authenticate()
        {
            var viewModel = new AuthenticateViewModel {ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateToken(string token, CancellationToken cancellationToken)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (id != null)
            {
                var response = await _validationService.ValidateUserAsync(new ValidationRequest { Token = token, UserId = id }, cancellationToken);
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
            if (user.RegistrationHistory.Count == 4) 
            {
                await _userService.CreateUserStepFiveAsync(id, true);
                return View();
            }

            return RedirectToAction("AuthFailed");
        }

        public IActionResult AuthFailed()
        {
            return View(new AuthenticateViewModel { ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId });
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
    }
}