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
       // private readonly IListViewBuilder<T> _listViewBuilder;    // Does this need a factory?

        public JobListViewComponent(IRequestService requestService, IAuthService authService, IGroupMemberService groupMemberService, IFilterService filterService, IAddressService addressService)
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

            switch (jobFilterRequest.JobSet.RequestType(), jobFilterRequest.JobSet)
            {
                case (RequestType.Task, _):
                    viewName = "JobList";
                    ListViewBuilder<JobHeader> listViewBuilder = new ListViewBuilder<JobHeader>(_requestService, _groupMemberService, _filterService, _addressService);
                    viewModel = await listViewBuilder.BuildList(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                case (RequestType.Shift, JobSet.UserOpenShifts):
                case (RequestType.Shift, JobSet.UserMyShifts):
                    viewName = "ShiftList";
                    ListViewBuilder<ShiftJob> listViewBuilder2 = new ListViewBuilder<ShiftJob>(_requestService, _groupMemberService, _filterService, _addressService);
                    viewModel = await listViewBuilder2.BuildList(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                case (RequestType.Shift, JobSet.GroupShifts):
                    viewName = "ShiftRequestList";
                    viewModel = await InvokeAsync_ShiftRequests(user, jobFilterRequest, hideFilterPanelCallback, noJobsCallback, cancellationToken);
                    break;

                default: throw new ArgumentException(message: $"Unexpected RequestType value: {jobFilterRequest.JobSet.RequestType()}");
            }

            return View(viewName, viewModel);
        }

        private async Task<ListViewModel<JobViewModel<RequestSummary>>> InvokeAsync_ShiftRequests(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var jobListViewModel = new ListViewModel<JobViewModel<RequestSummary>>();

            IEnumerable<RequestSummary> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.GroupShifts => await _requestService.GetGroupShiftRequestsAsync(jobFilterRequest.GroupId.Value, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            // Some jobs will already be filtered out in the Request Service, by jobFilterRequest.DueAfter and jobFilterRequest.DueBefore
            //  being passed through.  We probably therefore won't want to display the total number of Unfiltered items.
            jobListViewModel.UnfilteredItems = int.MaxValue;

            jobs = _filterService.SortAndFilterJobs(jobs, jobFilterRequest);

            jobListViewModel.FilteredItems = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            jobListViewModel.Items = await Task.WhenAll(jobs.Select(async a => new JobViewModel<RequestSummary>
            {
                Item = a,
                Location = (a.Shift != null ? new LocationWithDistance { LocationDetails = await _addressService.GetLocationDetails(a.Shift.Location, cancellationToken) } : null),
                UserRole = jobFilterRequest.JobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                JobListGroupId = jobFilterRequest.GroupId,
                UserHasRequiredCredentials = false,
                HighlightJob = a.JobSummaries.Select(j => (int?)j.JobID).Contains(jobFilterRequest.HighlightJobId) || a.RequestID.Equals(jobFilterRequest.HighlightRequestId),
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
    }
}
