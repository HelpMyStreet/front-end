using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
        private readonly IUserService _userService;

        public VolunteersApiController(IAuthService authService, IGroupMemberService groupMemberService, IUserService userService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-what-is-this-credential-popup")]
        public IActionResult GetWhatIsThisCredentialPopup(string g, string c, string i)
        {
            int groupId = Base64Utils.Base64DecodeToInt(g);
            int? credentialId = c != null ? Base64Utils.Base64DecodeToInt(c) : (int?)null;
            string item = i;

            return ViewComponent("WhatIsThisCredentialPopup", new { groupId, credentialId, item });
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

            var user = await _authService.GetCurrentUser(cancellationToken);

            DateTime? validUntil = assignCredentialsViewModel.ValidUntil == "Null" ? (DateTime?)null : DateTime.ParseExact(assignCredentialsViewModel.ValidUntil, DatePickerHelpers.DATE_PICKER_DATE_FORMAT, new CultureInfo("en-GB"));

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

        [AuthorizeAttributeNoRedirect]
        [HttpPost("update-biography")]
        public async Task<ActionResult<string>> UpdateBiography([FromBody] Dictionary<string, string> body, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            string answer = body["Biography"];

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var outcome = await _userService.AddBiography(user.ID, answer);

            switch (outcome)
            {
                case true:
                    return answer.ToHtmlSafeStringWithLineBreaks();
                default:
                    return StatusCode(500);
            }
        }
    }
}
