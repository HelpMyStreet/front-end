using System;
using HelpMyStreetFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(ILogger<RegistrationController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ActionResult Index(int step)
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = step
            });
        }
    }
}
