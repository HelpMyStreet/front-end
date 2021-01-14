using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class ShiftListViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public ShiftListViewComponent(IRequestService requestService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _requestService = requestService;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken)
        {
            var shiftListViewModel = new ListViewModel<ShiftViewModel>();

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (jobFilterRequest.JobSet == JobSet.GroupShifts)
            {
                if (!(await _groupMemberService.GetUserHasRole(user.ID, jobFilterRequest.GroupId.Value, GroupRoles.TaskAdmin, cancellationToken)))
                {
                    throw new UnauthorizedAccessException("User not authorized to view group tasks");
                }
            }

            IEnumerable<ShiftJob> jobs = jobFilterRequest.JobSet switch
            {
                JobSet.UserOpenShifts => await _requestService.GetOpenShiftsForUserAsync(user, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                JobSet.UserMyShifts => await _requestService.GetShiftsForUserAsync(user.ID, jobFilterRequest.DueAfter, jobFilterRequest.DueBefore, true, cancellationToken),
                JobSet.GroupShifts => await _requestService.GetGroupShiftRequestsAsync(jobFilterRequest.GroupId.Value, true, cancellationToken),
                _ => throw new ArgumentException(message: $"Unexpected JobSet value (for shifts): {jobFilterRequest.JobSet}", paramName: nameof(jobFilterRequest.JobSet))
            };

            if (jobs == null)
            {
                throw new Exception($"Failed to get shift jobs for user {user.ID}.  JobSet: {jobFilterRequest.JobSet}");
            }

            shiftListViewModel.UnfilteredItems = 999;
            shiftListViewModel.FilteredItems = jobs.Count();
            shiftListViewModel.ResultsToShowIncrement = jobFilterRequest.ResultsToShowIncrement;

            if (jobFilterRequest.ResultsToShow > 0)
            {
                jobs = jobs.Take(jobFilterRequest.ResultsToShow);
            }

            shiftListViewModel.Items = (await Task.WhenAll(jobs.Select(async a => new ShiftViewModel()
            {
                ShiftJob = a,
                UserRole = jobFilterRequest.JobSet == JobSet.GroupRequests ? RequestRoles.GroupAdmin : RequestRoles.Volunteer,
                UserHasRequiredCredentials = await _groupMemberService.GetUserHasCredentials(-1  /* ReferringGroupId */, a.Activity, user.ID, user.ID, cancellationToken),
                HighlightJob = a.JobID.Equals(jobFilterRequest.HighlightJobId),
            })));

            if (shiftListViewModel.UnfilteredItems == shiftListViewModel.FilteredItems && shiftListViewModel.UnfilteredItems <= 5)
            {
                hideFilterPanelCallback?.Invoke();

                if (shiftListViewModel.UnfilteredItems == 0)
                {
                    noJobsCallback?.Invoke();
                }
            }

            return View("ShiftList", shiftListViewModel);
        }
    }
}
