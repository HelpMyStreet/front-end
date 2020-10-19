using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Volunteers;

namespace HelpMyStreetFE.ViewComponents
{
    public class AssignCredentialsPopupViewComponent : ViewComponent
    {
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public AssignCredentialsPopupViewComponent(IAuthService authService, IGroupMemberService groupMemberService, IUserService userService, IGroupService groupService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int targetUserId, int groupId, int credentialId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if(!await _groupMemberService.GetUserHasRole(user.ID, groupId, GroupRoles.UserAdmin, cancellationToken))
            {
                throw new UnauthorizedAccessException("User does not have required role.");
            }

            if (!await _groupMemberService.GetUserHasRole(targetUserId, groupId, GroupRoles.Member, cancellationToken))
            {
                throw new UnauthorizedAccessException("Target user is not member of group.");
            }

            var viewModel = new AssignCredentialsViewModel()
            {
                TargetUser = await _userService.GetUserAsync(targetUserId, cancellationToken),
                Credential = await _groupService.GetGroupCredential(groupId, credentialId)
            };

            return View("AssignCredentialsPopup", viewModel);
        }
    }
}
