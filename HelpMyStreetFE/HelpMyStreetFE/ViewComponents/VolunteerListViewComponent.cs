﻿using HelpMyStreet.Utils.Enums;
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

            var group = await _groupService.GetGroupById(groupId, cancellationToken);
            var groupMembers = await _groupMemberService.GetAllGroupMembers(groupId, user.ID);
            var groupCompletedRequests = await _requestService.GetGroupRequestsAsync(groupId, true, cancellationToken);
            var groupCredentials = await _groupService.GetGroupCredentials(groupId);

            var getEachUser = groupMembers.Select(async (userInGroup) =>
            {
                return new VolunteerViewModel()
                {
                    Roles = userInGroup.GroupRoles,
                    User = await _userService.GetUserAsync(userInGroup.UserId, cancellationToken),
                    CompletedRequests = groupCompletedRequests.Where(j => j.JobStatus == JobStatuses.Done && j.VolunteerUserID == userInGroup.UserId).Count(),
                    Credentials = groupCredentials.Select(gc => new AnnotatedGroupCredential(gc, userInGroup.ValidCredentials)).ToList()
                };
            });

            var volunteerListViewModel = new VolunteerListViewModel
            {
                GroupId = group.GroupId,
                GroupCredentials = groupCredentials,
                Volunteers = (await Task.WhenAll(getEachUser)).Where(v => v.User != null),
                UserId = user.ID,
                UserHasEditRights = await _groupMemberService.GetUserHasRole(user.ID, groupId, GroupRoles.UserAdmin, cancellationToken),
            };


            return View("VolunteerList", volunteerListViewModel);
        }
    }
}
