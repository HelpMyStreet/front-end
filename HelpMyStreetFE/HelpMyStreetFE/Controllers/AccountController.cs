using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers {
  public class AccountController : Controller {

    public IActionResult Index () {
      return View ();
    }
  }
}