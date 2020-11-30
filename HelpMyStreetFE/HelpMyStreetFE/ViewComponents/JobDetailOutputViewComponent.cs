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
    public class JobDetailOutputViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        public JobDetailOutputViewComponent(IRequestService requestService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int JobID, int UserID, CancellationToken cancellationToken)
        {
            JobDetail jobDetails = await _requestService.GetJobDetailsAsync(JobID, UserID, cancellationToken);

            if (jobDetails == null)
            {
                throw new Exception($"Failed to retrieve job details for JobId {JobID}");
            }

            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                JobDetail = jobDetails,
                UserActingAsAdmin = false,
                GroupSupportActivityInstructions = await _groupService.GetGroupSupportActivityInstructions(jobDetails.JobSummary.ReferringGroupID, jobDetails.JobSummary.SupportActivity, cancellationToken),
            };

            return View(jobDetailViewModel);
        }
    }
}
