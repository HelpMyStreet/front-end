using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class AcceptRequestPopupViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Job job)
        {
            return View("AcceptRequestPopup", job);
        }
    }
}
