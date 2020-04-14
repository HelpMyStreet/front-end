using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View("404");
        }

        [Route("Error/{code:int}")]
        public IActionResult Error(int code)
        {
            // TODO: Add some logging in here - any non-404 error will hit this
                        
            return View("500");
        }
    }
}