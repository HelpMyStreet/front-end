using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class CredentialsRequiredPopupViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(CredentialsRequiredViewModel credentialsRequiredViewModel)
        {
            return View("CredentialsRequiredPopup", credentialsRequiredViewModel);
        }
    }
}
