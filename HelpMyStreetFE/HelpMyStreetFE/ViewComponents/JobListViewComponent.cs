using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;
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
    public class JobListViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public JobListViewComponent(IRequestService requestService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _requestService = requestService;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterRequest jobFilterRequest, Action<int> listLengthCallback, CancellationToken cancellationToken)
        {
            JobListViewModel jobListViewModel = new JobListViewModel();

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            if (jobFilterRequest.JobSet == JobSet.GroupRequests)
            {
                if (!(await _groupMemberService.GetUserHasRole(user.ID, jobFilterRequest.GroupId.Value, GroupRoles.TaskAdmin, cancellationToken)))
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

            jobs = _requestService.SortAndFilterJobs(jobs, jobFilterRequest);

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
                UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(a.ReferringGroupID, a.SupportActivity, user.ID, user.ID, cancellationToken)
            })));

            if (listLengthCallback != null)
            {
                listLengthCallback.Invoke(jobListViewModel.UnfilteredJobs);
            }

            return View("JobList", jobListViewModel);
        }
    }
}
