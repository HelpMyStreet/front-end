using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services;
using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Models.Home;
using System.Linq;
using HelpMyStreetFE.Services.Users;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Enums;
using System.Threading;

namespace HelpMyStreetFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly INewsTickersService _newsTickersService;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, INewsTickersService newsTickersService)
        {
            _logger = logger;
            _configuration = configuration;
            _newsTickersService = newsTickersService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get home");

            var model = new HomeViewModel
            {
                isLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated),
                NewsTickerMessages = await _newsTickersService.GetNewsTickerMessages(null, cancellationToken)
            };

            

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

        public IActionResult HealthCheck()
        {
            return Ok(DateTime.Now.FormatDate(DateTimeFormat.LongDateTimeFormat, false));
        }
    }
}
