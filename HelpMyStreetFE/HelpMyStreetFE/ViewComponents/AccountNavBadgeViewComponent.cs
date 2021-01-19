using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class AccountNavBadgeViewComponent : ViewComponent
    {
        private readonly IRequestService _requestService;
        private readonly IGroupMemberService _groupMemberService;

        public AccountNavBadgeViewComponent(IRequestService requestService, IGroupMemberService groupMemberService)
        {
            _requestService = requestService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IViewComponentResult> InvokeAsync(User user, MenuPage menuPage, string groupKey, string cssClass, CancellationToken cancellationToken)
        {
            var viewModel = new AccountNavBadgeViewModel()
            {
                CssClass = cssClass,
                MenuPage = menuPage,
                GroupKey = groupKey,
                Count = await GetCount(user, menuPage, groupKey, cancellationToken)
            };

            return View("AccountNavBadge", viewModel);
        }

        public async Task<int> GetCount(User user, MenuPage menuPage, string groupKey, CancellationToken cancellationToken)
        {
            if (menuPage == MenuPage.GroupRequests || menuPage == MenuPage.GroupShifts)
            {
                if (!await _groupMemberService.GetUserHasRole(user.ID, groupKey, GroupRoles.TaskAdmin, cancellationToken))
                {
                    return 0;
                }
            }

            int? count = menuPage switch
            {
                MenuPage.Group
                    => await GetCount(user, MenuPage.GroupRequests, groupKey, cancellationToken) + await GetCount(user, MenuPage.GroupShifts, groupKey, cancellationToken),
                MenuPage.GroupRequests
                    => (await _requestService.GetGroupRequestsAsync(groupKey, false, cancellationToken))?.Where(j => j.JobStatus.Incomplete())?.Count(),
                MenuPage.AcceptedRequests
                    => (await _requestService.GetJobsForUserAsync(user.ID, false, cancellationToken))?.Where(j => j.JobStatus == JobStatuses.InProgress)?.Count(),
                MenuPage.CompletedRequests
                    => (await _requestService.GetJobsForUserAsync(user.ID, false, cancellationToken))?.Where(j => j.JobStatus == JobStatuses.Done)?.Count(),
                MenuPage.OpenRequests
                    => (await _requestService.GetOpenJobsAsync(user, false, cancellationToken))?.Count(),
                MenuPage.OpenShifts
                    => (await _requestService.GetOpenShiftsForUserAsync(user, null, null, false, cancellationToken))?.Count(),
                MenuPage.MyShifts
                    => (await _requestService.GetShiftsForUserAsync(user.ID, null, null, false, cancellationToken))?.Count(),
                MenuPage.GroupShifts
                    => (await _requestService.GetGroupShiftRequestsAsync(groupKey, null, null, false, cancellationToken))?.Count(),
                _ => null
            };

            return count ?? 0;
        }
    }
}
