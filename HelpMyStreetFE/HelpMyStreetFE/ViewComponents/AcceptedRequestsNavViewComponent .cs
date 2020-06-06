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
    public class AcceptedRequestsNavViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;        
        public AcceptedRequestsNavViewComponent(IRequestService requestService)
        {
            _requestService = requestService;            
        }

 

        public async Task<IViewComponentResult> InvokeAsync(CountNavViewModel viewModel)
        {            
            if (((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated))
            {
                var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var jobs = await _requestService.GetJobsForUserAsync(id, HttpContext);
                viewModel.Count = jobs.Count();                               
            }                                
            return View(viewModel);
        }

    }
}
