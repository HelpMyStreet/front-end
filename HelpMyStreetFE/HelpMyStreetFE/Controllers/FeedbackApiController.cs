using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/feedback")]
    public class FeedbackApiController : Controller
    {
        private readonly IAuthService _authService;

        public FeedbackApiController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("post-task-feedback")]
        [AuthorizeAttributeNoRedirect]
        public IActionResult PostTaskFeedbackCapturePopup(string j, string r)
        {
            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return StatusCode(401);
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            return ViewComponent("FeedbackCapture", new { parameters = new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole } });
        }
    }
}
