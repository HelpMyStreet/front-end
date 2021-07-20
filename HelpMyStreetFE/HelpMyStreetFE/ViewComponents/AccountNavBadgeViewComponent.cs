using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
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
        private readonly IGroupService _groupService;

        public AccountNavBadgeViewComponent(IRequestService requestService, IGroupMemberService groupMemberService, IGroupService groupService)
        {
            _requestService = requestService;
            _groupMemberService = groupMemberService;
            _groupService = groupService;
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
            int groupId = 0;

            if (menuPage == MenuPage.GroupRequests || menuPage == MenuPage.GroupShifts)
            {
                var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
                groupId = group.GroupId;
                if (!await _groupMemberService.GetUserHasRole(user.ID, group.GroupId, GroupRoles.TaskAdmin, true, cancellationToken))
                {
                    return 0;
                }
                if (!group.ShiftsEnabled && menuPage == MenuPage.GroupShifts)
                {
                    return 0;
                }
                if (!group.TasksEnabled && menuPage == MenuPage.GroupRequests)
                {
                    return 0;
                }
            }

            try
            {
                int? count = menuPage switch
                {
                    MenuPage.Group
                        => await GetCount(user, MenuPage.GroupRequests, groupKey, cancellationToken) + await GetCount(user, MenuPage.GroupShifts, groupKey, cancellationToken),
                    MenuPage.GroupRequests
                        => (await _requestService.GetGroupRequestsAsync(groupId, false, cancellationToken))?.Count(r => !r.JobBasics.AllComplete()),
                    MenuPage.MyRequests
                        => (await _requestService.GetJobsForUserAsync(user.ID, false, cancellationToken))?.Where(j => j.JobStatus.Incomplete())?.Count(),
                    MenuPage.OpenRequests
                        => (await _requestService.GetOpenJobIdsAsync(user, false, cancellationToken))?.Count(),
                    MenuPage.OpenShifts
                        => (await _requestService.GetOpenShiftsForUserAsync(user, null, null, false, cancellationToken))?.Count(),
                    MenuPage.MyShifts
                        => (await _requestService.GetShiftsForUserAsync(user.ID, null, null, false, cancellationToken))?.Count(s => s.JobStatus.Incomplete()),
                    MenuPage.GroupShifts
                        => (await _requestService.GetGroupShiftRequestsAsync(groupId, null, null, false, cancellationToken))?.Count(r => !r.JobBasics.AllComplete()),
                    _ => null
                };

                return count ?? 0;
            }
            catch
            {
                // Skip badge if request service has been too slow to respond
                return 0;
            }
        }
    }
}
