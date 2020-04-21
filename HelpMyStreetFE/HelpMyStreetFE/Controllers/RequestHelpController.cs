using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{
    public class RequestHelpController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestHelpController> _logger;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;

        public RequestHelpController(ILogger<RequestHelpController> logger, IAddressService addressService, IUserService userService, IOptions<EmailConfig> app, IRequestHelpRepository requestHelpRepository)
        {
            appSettings = app;
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _addressService = addressService;
            _userService = userService;
        }

        public async Task<IActionResult> RequestHelp()
        {
            _logger.LogInformation("Get home");

            var reqs = new List<Task<int>>
            {
                _userService.GetStreetChampions(),
                _userService.GetStreetsCovered(),
                _userService.GetVolunteers(),
                _addressService.GetTotalStreets()
            };

            await Task.WhenAll(reqs);

            var model = new RequestHelpFormModel
            {
                NumStreetChampions = reqs[0].Result,
                NumStreetsCovered = reqs[1].Result,
                NumVolunteers = reqs[2].Result
            };

            int totalStreets = reqs[3].Result;

            model.PostCodesCoveredPercentage = (int)Math.Ceiling(100.0 * model.NumStreetsCovered / (totalStreets));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/RequestHelp/Send", Name = "RequestHelpSubmit")]
        public IActionResult SendEmail(RequestHelpFormModel requestHelpFormModel)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    _requestHelpRepository.PersistForm(requestHelpFormModel);

                    return View("Confirmation", requestHelpFormModel);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            requestHelpFormModel.HasErrors = true;
            return View("RequestHelp", requestHelpFormModel);
        }
    }
}