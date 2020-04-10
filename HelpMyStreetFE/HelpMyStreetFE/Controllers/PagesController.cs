using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class PagesController : Controller
    {
       
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Community()
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
    }
}