using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
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

        public YotiController(IOptions<YotiOptions> options, IValidationService validationService)
        {
            _options = options.Value;
            _validationService = validationService;
        }

        [AllowAnonymous] //Remove after development
        public IActionResult Authenticate()
        {
            var viewModel = new AuthenticateViewModel { ClientSdkId = _options.ClientSdkId, DomId = _options.DomId, ScenarioId = _options.ScenarioId };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ValidateToken(string token)
        {
            var i = HttpContext.User;
            //_validationService.ValidateUser();
            throw new NotImplementedException();
        }
    }
}