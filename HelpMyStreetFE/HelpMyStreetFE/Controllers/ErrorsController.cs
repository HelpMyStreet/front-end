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

        public ErrorsController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {

            return View("404");
        }

        [Route("Error/410")]
        [Route("link-expired")]
        public IActionResult Error410()
        {

            return View("410");
        }

        [Route("Error/{code:int}")]
        public IActionResult Error(int code)
        {           
            return View("500");
        }
    }
}