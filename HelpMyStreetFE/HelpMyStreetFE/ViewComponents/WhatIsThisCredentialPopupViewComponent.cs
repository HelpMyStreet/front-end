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
using HelpMyStreet.Contracts.GroupService.Response;

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

        public async Task<IViewComponentResult> InvokeAsync(int groupId, int? credentialId, string item, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (!await _groupMemberService.GetUserHasRole_Any(user.ID, groupId, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }, cancellationToken))
            {
                throw new UnauthorizedAccessException("User does not have required role.");
            }

            GroupCredential credential = credentialId.HasValue
                ? await _groupService.GetGroupCredential(groupId, credentialId.Value)
                : await GetItemDescription(groupId, item, cancellationToken);

            return View("WhatIsThisCredentialPopup", credential);
        }

        private async Task<GroupCredential> GetItemDescription(int groupId, string item, CancellationToken cancellationToken)
        {
            if (item == "completed-requests")
            {
                var group = await _groupService.GetGroupById(groupId, cancellationToken);

                return new GroupCredential
                {
                    Name = "Completed Requests",
                    WhatIsThis = $"This is the number of requests completed by the user for **{group.GroupName}**."
                };
            }
            throw new ArgumentException($"Unexpected item {item}", item);
        }
    }
}
