using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobDetailViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        public JobDetailViewComponent(IRequestService requestService, IUserService userService, IGroupService groupService)
        {
            _requestService = requestService;
            _userService = userService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int JobID, User user, CancellationToken cancellationToken)
        {
            var jobDetails = await _requestService.GetJobDetailsAsync(JobID, user.ID, cancellationToken);

            if (jobDetails == null)
            {
                throw new Exception($"Failed to retrieve job details for JobId {JobID}");
            }

            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel()
            {
                JobDetail = jobDetails,
                UserIsVerified = user.IsVerified ?? false,
                UserActingAsAdmin = await _groupService.GetUserHasRole(user.ID, jobDetails.JobSummary.ReferringGroupID.Value, GroupRoles.TaskAdmin)
            };

            return View("JobDetail", jobDetailViewModel);
        }
    }
}
