using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IAuthService authService, IFeedbackService feedbackService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public IActionResult PostTaskFeedbackCapture(string j, string r, string f)
        {
            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return Redirect("/Error/401");
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
            FeedbackRating feedbackRating = (FeedbackRating)Base64Utils.Base64DecodeToInt(f);

            return View(new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole, FeedbackRating = feedbackRating });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostTaskFeedbackCapture(string j, string r, FeedbackCaptureEditModel model, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return Redirect("/Error/401");
            }
            if (!ModelState.IsValid)
            {
                throw new Exception($"Invalid model state in PostTaskFeedbackCapture for job {jobId}");
            }

            model.JobId = jobId;
            model.RoleSubmittingFeedback = requestRole;

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            var result = await _feedbackService.PostRecordFeedback(user, model);

            return View("PostTaskFeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = result });
        }

    }
}
