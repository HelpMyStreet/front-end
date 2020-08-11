using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobListViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;

        public JobListViewComponent(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<IViewComponentResult> InvokeAsync(MenuPage menuPage, int? groupId)
        {
            IEnumerable<JobSummary> jobs = null;
            switch (menuPage)
            {
                case MenuPage.GroupRequests:
                    jobs =await _requestService.GetGroupRequestsAsync(groupId.Value);
                    break;
                case MenuPage.OpenRequests:
                    //jobs = await _requestService.GetOpenJobsAsync()
                    break;
                case MenuPage.CompletedRequests:
                    break;
                case MenuPage.AcceptedRequests:
                    //jobs = await _requestService.GetJobsForUserAsync()
                    break;
            }


            return View("JobList", jobs);
        }
    }
}
