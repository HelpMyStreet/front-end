using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class ReportViewComponent : ViewComponent
    {
        public ReportViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
        {
            


            return View("Report", new ReportViewModel());
        }
    }
}
