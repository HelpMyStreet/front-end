using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
  public class PagesController : Controller
  {
    public IActionResult Community()
    {
      return View();
    }
  
    public IActionResult AboutUs()
    {
      return View();
    }
    public IActionResult PrivacyPolicy()
    {
      return View();
    }
  }
}