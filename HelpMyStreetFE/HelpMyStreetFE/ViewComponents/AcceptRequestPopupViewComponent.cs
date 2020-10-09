using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class AcceptRequestPopupViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(JobStatusChangePopupViewModel jobStatusChangePopupViewModel)
        {
            return View("AcceptRequestPopup", jobStatusChangePopupViewModel);
        }
    }
}
