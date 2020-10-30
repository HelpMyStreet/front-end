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
using HelpMyStreetFE.Models;

namespace HelpMyStreetFE.ViewComponents
{
    public class WhatIsThisCredentialPopupViewComponent : ViewComponent
    {
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupService _groupService;

        public WhatIsThisCredentialPopupViewComponent(IAuthService authService, IGroupMemberService groupMemberService, IGroupService groupService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int groupId, int credentialId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (!await _groupMemberService.GetUserHasRole(user.ID, groupId, GroupRoles.UserAdmin, cancellationToken))
            {
                throw new UnauthorizedAccessException("User does not have required role.");
            }

            var credential = await _groupService.GetGroupCredential(groupId, credentialId);

            return View("WhatIsThisCredentialPopup", credential);
        }
    }
}
