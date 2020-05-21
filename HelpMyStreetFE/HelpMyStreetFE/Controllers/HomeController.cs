using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Models.Home;

namespace HelpMyStreetFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IAddressService addressService, IUserService userService, IConfiguration configuration)
        {
            _logger = logger;
            _addressService = addressService;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
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

            var model = new HomeViewModel
            {
                NumStreetChampions = reqs[0].Result,
                NumStreetsCovered = reqs[1].Result,
                NumVolunteers = reqs[2].Result,                
            };

            int totalStreets = reqs[3].Result;

            model.PostCodesCoveredPercentage = (int)Math.Ceiling(100.0 * model.NumStreetsCovered / (totalStreets));

            return View(model);
        }

        
        [HttpGet]
        public async Task<IActionResult> FirebaseAccountAction(string mode, string oobCode, string apiKey, string continueUrl)
        {
            switch (mode)
            {
                case "resetPassword":
                    return RedirectToAction("ResetPassword", new { oobCode = oobCode });
                default:
                    return RedirectToAction("index");
            }            
        }

        [HttpGet]
        public async Task<IActionResult> ForgottenPassword()
        {     
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string oobCode)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {        
                ActionCode = oobCode

            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }       
    }
}
