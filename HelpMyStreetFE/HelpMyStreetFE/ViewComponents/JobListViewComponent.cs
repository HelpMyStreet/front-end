using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobListViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;

        public JobListViewComponent(IRequestService requestService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterRequest jobFilterRequest, Action emptyListCallback, CancellationToken cancellationToken)
        {
            User user = HttpContext.Session.GetObjectFromJson<User>("User");

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            if (jobFilterRequest.JobSet == JobSet.GroupRequests)
            {
                if (!(await _groupService.GetUserGroupRoles(user.ID))
                    .Where(g => g.GroupId == jobFilterRequest.GroupId.Value)
                    .FirstOrDefault().UserRoles.Contains(GroupRoles.TaskAdmin))
                {
                    throw new UnauthorizedAccessException("User not authorized to view group tasks");
                }
            }

            IEnumerable<JobSummary> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.GroupRequests => await _requestService.GetGroupRequestsAsync(jobFilterRequest.GroupId.Value, cancellationToken),
                JobSet.UserOpenRequests_MatchingCriteria => (await _requestService.GetOpenJobsAsync(user, cancellationToken)).CriteriaJobs,
                JobSet.UserOpenRequests_NotMatchingCriteria => (await _requestService.GetOpenJobsAsync(user, cancellationToken)).OtherJobs,
                JobSet.UserAcceptedRequests => (await _requestService.GetJobsForUserAsync(user.ID, cancellationToken)).Where(j => j.JobStatus == JobStatuses.InProgress),
                JobSet.UserCompletedRequests => (await _requestService.GetJobsForUserAsync(user.ID, cancellationToken)).Where(j => j.JobStatus == JobStatuses.Done || j.JobStatus == JobStatuses.Cancelled),
                _ => throw new ArgumentException(message: "Invalid JobSet value", paramName: nameof(jobFilterRequest.JobSet))
            };

            jobs = _requestService.FilterJobs(jobs, jobFilterRequest);

            if (jobs.Count() == 0 && emptyListCallback != null) { emptyListCallback.Invoke(); }

            var jobs2 = jobs.Select(a => new JobViewModel()
            {
                JobSummary = a,
                UserActingAsAdmin = jobFilterRequest.JobSet == JobSet.GroupRequests,
                UserIsVerified = user.IsVerified ?? false
            });

            return View("JobList", jobs2);
        }
    }
}
