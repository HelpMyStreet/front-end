using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HelpMyStreetFE.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly IConfiguration _configuration;
        public ErrorsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View("404", model);
        }

        [Route("Error/{code:int}")]
        public IActionResult Error(int code)
        {
            // TODO: Add some logging in here - any non-404 error will hit this.
            BasePageViewModel model = new BasePageViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };

            return View("500", model);
        }
    }
}