using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers {
  public class AccountController : Controller {

    public IActionResult Index () {
      return View ();
    }
  }
}