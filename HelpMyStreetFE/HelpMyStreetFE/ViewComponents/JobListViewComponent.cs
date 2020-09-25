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
        private readonly IAuthService _authService;

        public JobListViewComponent(IRequestService requestService, IGroupService groupService, IAuthService authService)
        {
            _requestService = requestService;
            _groupService = groupService;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterRequest jobFilterRequest, Action emptyListCallback, CancellationToken cancellationToken)
        {
            JobListViewModel jobListViewModel = new JobListViewModel();

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            if (jobFilterRequest.JobSet == JobSet.GroupRequests)
            {
                if (!(await _groupService.GetUserHasRole(user.ID, jobFilterRequest.GroupId.Value, GroupRoles.TaskAdmin, cancellationToken)))
                {
                    throw new UnauthorizedAccessException("User not authorized to view group tasks");
                }
            }

            IEnumerable<JobHeader> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.GroupRequests => await _requestService.GetGroupRequestsAsync(jobFilterRequest.GroupId.Value, true, cancellationToken),
                JobSet.UserOpenRequests_MatchingCriteria => _requestService.SplitOpenJobs(user, await _requestService.GetOpenJobsAsync(user, true, cancellationToken))?.CriteriaJobs,
                JobSet.UserOpenRequests_NotMatchingCriteria => _requestService.SplitOpenJobs(user, await _requestService.GetOpenJobsAsync(user, true, cancellationToken))?.OtherJobs,
                JobSet.UserAcceptedRequests => (await _requestService.GetJobsForUserAsync(user.ID, true, cancellationToken)).Where(j => j.JobStatus == JobStatuses.InProgress),
                JobSet.UserCompletedRequests => (await _requestService.GetJobsForUserAsync(user.ID, true, cancellationToken)).Where(j => j.JobStatus == JobStatuses.Done || j.JobStatus == JobStatuses.Cancelled),
                _ => throw new ArgumentException(message: $"Invalid JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            jobListViewModel.UnfilteredJobs = jobs.Count();

            jobs = _requestService.FilterJobs(jobs, jobFilterRequest);

            jobListViewModel.FilteredJobs = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            jobListViewModel.Jobs = (await Task.WhenAll(jobs.Select(async a => new JobViewModel()
            {
                JobHeader = a,                
                UserActingAsAdmin = jobFilterRequest.JobSet == JobSet.GroupRequests,
                UserIsVerified = user.IsVerified ?? false,
                ReferringGroup = a.ReferringGroupID.HasValue ? (await _groupService.GetGroupById(a.ReferringGroupID.Value, cancellationToken))?.GroupName : ""
            })));

            if (jobListViewModel.UnfilteredJobs == 0 && emptyListCallback != null)
            {
                emptyListCallback.Invoke();
            }

            return View("JobList", jobListViewModel);
        }
    }
}
