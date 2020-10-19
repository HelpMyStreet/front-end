using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/volunteers")]
    [ApiController]
    public class VolunteersApiController : Controller
    {
        private readonly IAuthService _authService;

        public VolunteersApiController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-assign-credential-popup")]
        public async Task<IActionResult> GetAssignCredentialPopup(string u, string g, string c)
        {
            int targetUserId = Base64Utils.Base64DecodeToInt(u);
            int groupId = Base64Utils.Base64DecodeToInt(g);
            int credentialId = Base64Utils.Base64DecodeToInt(c);

            return ViewComponent("AssignCredentialsPopup", new { targetUserId, groupId, credentialId });
        }
    }
}
