using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobFilterPanelViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(JobFilterViewModel jobFilterViewModel)
        {
            return View("JobFilterPanel", jobFilterViewModel);
        }

    }
}
