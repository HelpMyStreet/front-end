using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Services.Requests
{
    public class ListViewBuilder<T> : IListViewBuilder<T> where T : JobBasic
    {
        private readonly IRequestService _requestService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IFilterService _filterService;
        private readonly IAddressService _addressService;

        public ListViewBuilder(IRequestService requestService, IGroupMemberService groupMemberService, IFilterService filterService, IAddressService addressService)
        {
            _requestService = requestService;
            _groupMemberService = groupMemberService;
            _filterService = filterService;
            _addressService = addressService;
        }

        public async Task<ListViewModel<JobViewModel<T>>> BuildList(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var jobListViewModel = new ListViewModel<JobViewModel<T>>();

            IEnumerable<T> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.GroupRequests => (IEnumerable<T>)await _requestService.GetGroupRequestsAsync(jobFilterRequest.GroupId.Value, true, cancellationToken),
                JobSet.UserOpenRequests_MatchingCriteria => (IEnumerable<T>)_requestService.SplitOpenJobs(user, await _requestService.GetOpenJobsAsync(user, true, cancellationToken))?.CriteriaJobs,
                JobSet.UserOpenRequests_NotMatchingCriteria => (IEnumerable<T>)_requestService.SplitOpenJobs(user, await _requestService.GetOpenJobsAsync(user, true, cancellationToken))?.OtherJobs,
                JobSet.UserMyRequests => (IEnumerable<T>)await _requestService.GetJobsForUserAsync(user.ID, true, cancellationToken),
                
                JobSet.UserOpenShifts => (IEnumerable<T>)await _requestService.GetOpenShiftsForUserAsync(user, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                JobSet.UserMyShifts => (IEnumerable<T>)await _requestService.GetShiftsForUserAsync(user.ID, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            jobListViewModel.UnfilteredItems = jobs.Count();

            jobs = (IEnumerable<T>)_filterService.SortAndFilterJobs(jobs, jobFilterRequest);

            jobListViewModel.FilteredItems = jobs.Count();
            jobListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            jobListViewModel.Items = await Task.WhenAll(jobs.Select(async a => new JobViewModel<T>
            {
                Item = a,
                UserRole = jobFilterRequest.JobSet.GroupAdminView() ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(a.ReferringGroupID, a.SupportActivity, user.ID, user.ID, cancellationToken),
                HighlightJob = a.JobID.Equals(jobFilterRequest.HighlightJobId),
            }));

            if (jobFilterRequest.JobSet.RequestType().Equals(RequestType.Shift))
            {
                await AttachLocationDetails((IEnumerable<JobViewModel<ShiftJob>>)jobListViewModel.Items, user, cancellationToken);

                if (jobFilterRequest.DueBefore != null || jobFilterRequest.DueAfter != null)
                {
                    // Some jobs may have been filtered out by date
                    jobListViewModel.UnfilteredItems = int.MaxValue;
                }
            }

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

        private async Task AttachLocationDetails(IEnumerable<JobViewModel<ShiftJob>> jobs, User user, CancellationToken cancellationToken)
        {
            IEnumerable<LocationWithDistance> userLocationDetails = await _addressService.GetLocationDetailsForUser(user, cancellationToken);
      
            foreach (JobViewModel<ShiftJob> job in jobs)
            {
                job.Location = userLocationDetails.FirstOrDefault(l => l.Location == job.Item.Location);
            }
        }
    }
}
