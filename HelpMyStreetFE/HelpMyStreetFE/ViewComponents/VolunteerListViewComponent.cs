using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Services;
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

        public VolunteerListViewComponent(IGroupService groupService, IUserService userService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _groupService = groupService;
            _userService = userService;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int groupId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }


            var groupMembers = await _groupMemberService.GetGroupMembers(groupId, user.ID, cancellationToken);


            var getEachUser = groupMembers.Select(async (userGroup) =>
            {
                return new VolunteerViewModel()
                {
                    Roles = userGroup.UserRoles,
                    User = await _userService.GetUserAsync(userGroup.UserId, cancellationToken)
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
