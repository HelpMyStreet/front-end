using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services;
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
        private readonly IFilterService _filterService;
        private readonly IAddressService _addressService;

        public JobListViewComponent(
            IRequestService requestService, 
            IAuthService authService, 
            IGroupMemberService groupMemberService, 
            IFilterService filterService, 
            IAddressService addressService)
        {
            _requestService = requestService;
            _authService = authService;
            _groupMemberService = groupMemberService;
            _filterService = filterService;
            _addressService = addressService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (jobFilterRequest.JobSet.GroupAdminView())
            {
                if (!(await _groupMemberService.GetUserHasRole(user.ID, jobFilterRequest.GroupId.Value, GroupRoles.TaskAdmin, true, cancellationToken)))
                {
                    throw new UnauthorizedAccessException("User not authorized to view group tasks");
                }
            }

            string viewName;
            object viewModel;

            // TODO: Consolidate 3 methods
            switch (jobFilterRequest.JobSet.RequestType(), jobFilterRequest.JobSet)
            {
                case (RequestType.Shift, JobSet.GroupShifts):
                case (RequestType.Task, JobSet.GroupRequests):
                    viewName = "ShiftRequestList";
                    viewModel = await InvokeAsync_Requests(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                case (RequestType.Task, JobSet.UserMyRequests):
                    viewName = "MyRequestsList";
                    viewModel = await InvokeAsync_Requests(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                case (RequestType.Task, JobSet.UserOpenRequests_MatchingCriteria):
                case (RequestType.Task, JobSet.UserOpenRequests_NotMatchingCriteria):
                    viewName = "OpenJobList";
                    viewModel = await InvokeAsync_OpenJobs(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                case (RequestType.Shift, _):
                    viewName = "ShiftList";
                    viewModel = await InvokeAsync_ShiftJobs(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                default: throw new ArgumentException(message: $"Unexpected RequestType value: {jobFilterRequest.JobSet.RequestType()}");
            }

            return View(viewName, viewModel);
        }

        private async Task<ListViewModel<JobViewModel<IEnumerable<JobDetail>>>> InvokeAsync_OpenJobs(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var jobListViewModel = new ListViewModel<JobViewModel<IEnumerable<JobDetail>>>();

            IEnumerable<IEnumerable<JobSummary>> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.UserOpenRequests_MatchingCriteria => _requestService.SplitOpenJobs(user, await _requestService.GetDedupedOpenJobsForUserFromRepo(user, true, cancellationToken))?.CriteriaJobs,
                JobSet.UserOpenRequests_NotMatchingCriteria => _requestService.SplitOpenJobs(user, await _requestService.GetDedupedOpenJobsForUserFromRepo(user, true, cancellationToken))?.OtherJobs,
                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            jobListViewModel.UnfilteredItems = jobs.Count();

            jobs = await _filterService.SortAndFilterOpenJobs(jobs, jobFilterRequest, cancellationToken);

            jobListViewModel.FilteredItems = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            jobListViewModel.Items = await Task.WhenAll(jobs.Select(async a => new JobViewModel<IEnumerable<JobDetail>>()
            {
                Item = a.Select(j => new JobDetail(j)),
                UserRole = jobFilterRequest.JobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                Location = null,//await _addressService.GetLocationWithDistance(a, cancellationToken),
                JobListGroupId = jobFilterRequest.GroupId,
                UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(a.First().ReferringGroupID, a.First().SupportActivity, user.ID, user.ID, cancellationToken),
                HighlightJob = a.Any(j => j.JobID.Equals(jobFilterRequest.HighlightJobId)) || a.First().RequestID.Equals(jobFilterRequest.HighlightRequestId),
            })); ;

            if (jobListViewModel.UnfilteredItems == jobListViewModel.FilteredItems && jobListViewModel.UnfilteredItems <= 5)
            {
                hideFilterPanelCallback?.Invoke();

                if (jobListViewModel.UnfilteredItems == 0)
                {
                    noJobsCallback?.Invoke();
                }
            }

            return jobListViewModel;
        }

        private async Task<ListViewModel<JobViewModel<ShiftJob>>> InvokeAsync_ShiftJobs(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var jobListViewModel = new ListViewModel<JobViewModel<ShiftJob>>();

            IEnumerable<ShiftJob> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.UserOpenShifts => await GetOpenShiftsForUserAsync(user, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, cancellationToken),
                JobSet.UserMyShifts => await GetShiftsForUserAsync(user, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            // Some jobs will already be filtered out in the Request Service, by jobFilterRequest.DueAfter and jobFilterRequest.DueBefore
            //  being passed through.  We probably therefore won't want to display the total number of Unfiltered items.
            jobListViewModel.UnfilteredItems = int.MaxValue;

            jobs = _filterService.SortAndFilterShiftJobs(jobs, jobFilterRequest);

            jobListViewModel.FilteredItems = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            var userLocationDetails = await _addressService.GetLocationDetailsForUser(user, cancellationToken);

            jobListViewModel.Items = await Task.WhenAll(jobs.Select(async a => new JobViewModel<ShiftJob>()
            {
                Item = a,
                Location = null, //await _addressService.GetLocationWithDistance(a, cancellationToken),
                UserRole = jobFilterRequest.JobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                JobListGroupId = jobFilterRequest.GroupId,
                UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(a.ReferringGroupID, a.SupportActivity, user.ID, user.ID, cancellationToken),
                HighlightJob = a.JobID.Equals(jobFilterRequest.HighlightJobId) || a.RequestID.Equals(jobFilterRequest.HighlightRequestId),
            }));

            if (jobListViewModel.UnfilteredItems == jobListViewModel.FilteredItems && jobListViewModel.UnfilteredItems <= 5)
            {
                hideFilterPanelCallback?.Invoke();

                if (jobListViewModel.UnfilteredItems == 0)
                {
                    noJobsCallback?.Invoke();
                }
            }

            return jobListViewModel;
        }

        private async Task<ListViewModel<JobViewModel<RequestSummary>>> InvokeAsync_Requests(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var jobListViewModel = new ListViewModel<JobViewModel<RequestSummary>>();

            IEnumerable<RequestSummary> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.GroupRequests => await _requestService.GetGroupRequestsAsync(jobFilterRequest.GroupId.Value, true, cancellationToken),
                JobSet.GroupShifts => await _requestService.GetGroupShiftRequestsAsync(jobFilterRequest.GroupId.Value, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                JobSet.UserMyRequests => await _requestService.GetRequestsForUserAsync(user.ID, true, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            // Some jobs will already be filtered out in the Request Service, by jobFilterRequest.DueAfter and jobFilterRequest.DueBefore
            //  being passed through.  We probably therefore won't want to display the total number of Unfiltered items.
            jobListViewModel.UnfilteredItems = int.MaxValue;

            jobs = await _filterService.SortAndFilterRequests(jobs, jobFilterRequest, cancellationToken);

            jobListViewModel.FilteredItems = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            jobListViewModel.Items = await Task.WhenAll(jobs.Select(async a => new JobViewModel<RequestSummary>
            {
                Item = a,
                Location = await _addressService.GetLocationWithDistance(a, cancellationToken),
                User = user,
                UserRole = jobFilterRequest.JobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                JobListGroupId = jobFilterRequest.GroupId,
                UserHasRequiredCredentials = false,
                HighlightJob = a.JobBasics.Select(j => (int?)j.JobID).Contains(jobFilterRequest.HighlightJobId) || a.RequestID.Equals(jobFilterRequest.HighlightRequestId),
            }));

            if (jobListViewModel.UnfilteredItems == jobListViewModel.FilteredItems && jobListViewModel.UnfilteredItems <= 5)
            {
                hideFilterPanelCallback?.Invoke();

                if (jobListViewModel.UnfilteredItems == 0)
                {
                    noJobsCallback?.Invoke();
                }
            }

            return jobListViewModel;
        }

        private async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
        {
            var jobs = await _requestService.GetOpenShiftsForUserAsync(user, dateFrom, dateTo, true, cancellationToken);
            return jobs;
        }

        private async Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken)
        {
            var jobs = await _requestService.GetShiftsForUserAsync(user.ID, dateFrom, dateTo, true, cancellationToken);
            return jobs;
        }
    }
}
