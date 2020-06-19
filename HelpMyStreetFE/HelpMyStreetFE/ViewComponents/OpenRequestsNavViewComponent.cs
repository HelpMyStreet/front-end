using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class OpenRequestsNavViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IOptions<RequestSettings> _requestSettings;
       
        public OpenRequestsNavViewComponent(IRequestService requestService, IOptions<RequestSettings> requestSettings)
        {
            _requestService = requestService;
            _requestSettings = requestSettings;
        }



        public async Task<IViewComponentResult> InvokeAsync(CountNavViewModel viewModel)
        {            
            if (((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated))
            {                
                var user = HttpContext.Session.GetObjectFromJson<User>("User");
                var jobs = await _requestService.GetOpenJobsAsync(_requestSettings.Value.OpenRequestsRadius, _requestSettings.Value.MaxNonCriteriaOpenJobsToDisplay, user, HttpContext);
                viewModel.Count = jobs.CriteriaJobs.Count() + jobs.OtherJobs.Count();
            }
            return View(viewModel);
        }


    }
}
