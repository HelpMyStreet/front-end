using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Volunteers;
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
    public class VolunteerListViewComponent: ViewComponent
    {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IRequestService _requestService;

        public VolunteerListViewComponent(IGroupService groupService, IUserService userService, IAuthService authService, IGroupMemberService groupMemberService, IRequestService requestService)
        {
            _groupService = groupService;
            _userService = userService;
            _authService = authService;
            _groupMemberService = groupMemberService;
            _requestService = requestService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int groupId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            var groupMembers = await _groupMemberService.GetGroupMembers(groupId, user.ID, cancellationToken);
            var groupCompletedRequests = await _requestService.GetGroupRequestsAsync(groupId, true, cancellationToken);

            var getEachUser = groupMembers.Select(async (userGroup) =>
            {
                return new VolunteerViewModel()
                {
                    Roles = userGroup.UserRoles,
                    User = await _userService.GetUserAsync(userGroup.UserId, cancellationToken),
                    CompletedRequests = groupCompletedRequests.Where(j => j.JobStatus == JobStatuses.Done && j.VolunteerUserID == userGroup.UserId).Count()
                };
            });

            VolunteerListViewModel volunteerListViewModel = new VolunteerListViewModel
            {
                Volunteers = (await Task.WhenAll(getEachUser)).Where(v => v.User != null)
            };


            return View("VolunteerList", volunteerListViewModel);
        }
    }
}
