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
    public class JobDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        public JobDetailViewComponent(IRequestService requestService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int jobId, User user, JobSet jobSet, CancellationToken cancellationToken)
        {
            JobDetail jobDetails = jobSet switch
            {
                JobSet.GroupRequests => await _requestService.GetJobDetailsAsync(jobId, user.ID, cancellationToken),
                JobSet.UserCompletedRequests => await _requestService.GetJobDetailsAsync(jobId, user.ID, cancellationToken),
                JobSet.UserAcceptedRequests => await _requestService.GetJobDetailsAsync(jobId, user.ID, cancellationToken),
                _ => new JobDetail() { JobSummary = await _requestService.GetJobSummaryAsync(jobId, cancellationToken) }
            };

            if (jobDetails == null)
            {
                throw new Exception($"Failed to retrieve job details for JobId {jobId}");
            }

            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                JobDetail = jobDetails,
                UserActingAsAdmin = jobSet == JobSet.GroupRequests,
            };

            return View("JobDetail", jobDetailViewModel);
        }
    }
}
