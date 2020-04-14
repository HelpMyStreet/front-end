using HelpMyStreetFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HelpMyStreetFE.Controllers
{
    public class PagesController : Controller
    {
        private readonly IConfiguration _configuration;
        public PagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public IActionResult AboutUs()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);            
        }
        public IActionResult Community()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult PrivacyPolicy()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult Terms()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult Resources()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult Questions()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult ContactUs()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }
        public IActionResult StyleGuide()
        {
            return View();
        }
    }
}