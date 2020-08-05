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

namespace HelpMyStreetFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IFeedbackRepository _feedbackRepository;

        public HomeController(ILogger<HomeController> logger, IAddressService addressService, IUserService userService, IConfiguration configuration, IFeedbackRepository feedbackRepository)
        {
            _logger = logger;
            _addressService = addressService;
            _userService = userService;
            _configuration = configuration;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Get home");

            bool testBanner = false;
            string strTestBanner = _configuration["TestBanner"];
            if (!string.IsNullOrEmpty(strTestBanner))
            {
                testBanner = Convert.ToBoolean(_configuration["TestBanner"]);
            }

            var model = new HomeViewModel
            {
                isLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated),
                TestBanner = testBanner,
                FeedbackMessages = await _feedbackRepository.GetFeedback()
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
            return Ok(DateTime.Now.ToString("dd-MM-yyy hh:mmtt"));
        }
    }
}
