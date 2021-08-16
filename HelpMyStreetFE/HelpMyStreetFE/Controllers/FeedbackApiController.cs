using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackApiController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;

        public FeedbackApiController(IAuthService authService, IFeedbackService feedbackService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
        }

        [HttpGet("get-post-task-feedback-popup")]
        [AuthorizeAttributeNoRedirect]
        public IActionResult PostTaskFeedbackCapturePopup(string j, string r)
        {
            if (!_authService.GetUrlIsSessionAuthorised())
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            return ViewComponent("FeedbackCapture", new { parameters = new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole, RenderAsPopup = true } });
        }

        [Route("put-feedback")]
        [AuthorizeAttributeNoRedirect]
        public async Task<IActionResult> PutFeedback(string j, string r, [FromBody] CapturedFeedback model, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            if (!_authService.GetUrlIsSessionAuthorised())
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            if (!ModelState.IsValid)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            model.JobId = jobId;
            model.RoleSubmittingFeedback = requestRole;

            var user = await _authService.GetCurrentUser(cancellationToken);
            var result = await _feedbackService.PostRecordFeedback(user, model);

            if (result == Result.Success || result == Result.Failure_FeedbackAlreadyRecorded)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [Route("get-feedback-thanks-popup")]
        [AuthorizeAttributeNoRedirect]
        public IActionResult FeedbackThanksPopup([FromBody] CapturedFeedback capturedFeedback)
        {
            return ViewComponent("FeedbackCaptureThanks", capturedFeedback);
        }
    }
}
