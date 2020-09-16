using HelpMyStreetFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HelpMyStreetFE.Controllers
{
    public class PagesController : Controller
    {
        public PagesController()
        {   
        }
       
        public IActionResult AboutUs()
        {
            return View();            
        }
        public IActionResult CommunityOrganisers()
        {    
            return View();
        }
        public IActionResult PrivacyPolicy()
        {    
            return View();
        }
        public IActionResult Terms()
        {       
            return View();
        }
        public IActionResult Resources()
        {    
            return View();
        }
        public IActionResult Questions()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult StyleGuide()
        {
            return View();
        }
        public IActionResult CaseStudies()
        {
            return View();
        }
        public IActionResult RequestHelp()
        {
            return View("~/Views/RequestHelp/RequestHelp.cshtml");
        }

    }
}