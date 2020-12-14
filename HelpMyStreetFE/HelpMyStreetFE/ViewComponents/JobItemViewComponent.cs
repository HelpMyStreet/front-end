using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobItemViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        public JobItemViewComponent(IRequestService requestService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, User user, CancellationToken cancellationToken)
        {
            JobSummary jobSummary = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);

            JobViewModel jobViewModel = new JobViewModel()
            {
                JobHeader = jobSummary,
                UserHasRequiredCredentials = true,
                HighlightJob = false,
                UserRole = HelpMyStreet.Utils.Enums.RequestRoles.Volunteer
            };

            return View(jobViewModel);
        }
    }
}
