﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/volunteers")]
    [ApiController]
    public class VolunteersApiController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public VolunteersApiController(IAuthService authService, IGroupMemberService groupMemberService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-what-is-this-credential-popup")]
        public IActionResult GetAssignCredentialPopup(string g, string c)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            int credentialId = Base64Utils.Base64DecodeToInt(c);

            return ViewComponent("WhatIsThisCredentialPopup", new { groupId, credentialId });
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-assign-credential-popup")]
        public IActionResult GetAssignCredentialPopup(string u, string g, string c)
        {
            int targetUserId = Base64Utils.Base64DecodeToInt(u);
            int groupId = Base64Utils.Base64DecodeToInt(g);
            int credentialId = Base64Utils.Base64DecodeToInt(c);

            return ViewComponent("AssignCredentialsPopup", new { targetUserId, groupId, credentialId });
        }

        [AuthorizeAttributeNoRedirect]
        [Route("put-volunteer-credential")]
        public async Task<bool> PutVolunteerCredential(string u, string g, string c, [FromBody] AssignCredentialsViewModel assignCredentialsViewModel, CancellationToken cancellationToken)
        {
            int targetUserId = Base64Utils.Base64DecodeToInt(u);
            int groupId = Base64Utils.Base64DecodeToInt(g);
            int credentialId = Base64Utils.Base64DecodeToInt(c);

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            DateTime? validUntil = assignCredentialsViewModel.ValidUntil == "Null" ? (DateTime?)null : DateTime.Parse(assignCredentialsViewModel.ValidUntil);

            PutGroupMemberCredentialsRequest putGroupMemberCredentialsRequest = new PutGroupMemberCredentialsRequest()
            {
                AuthorisedByUserID = user.ID,
                UserId = targetUserId,
                GroupId = groupId,
                CredentialId = credentialId,
                Reference = assignCredentialsViewModel.Reference,
                Notes = assignCredentialsViewModel.Notes,
                ValidUntil = validUntil,
            };

            return await _groupMemberService.PutGroupMemberCredentials(putGroupMemberCredentialsRequest);
        }
    }
}