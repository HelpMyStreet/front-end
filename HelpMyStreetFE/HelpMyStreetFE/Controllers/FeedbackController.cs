﻿using System;
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
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;
        private readonly IRequestService _requestService;

        public FeedbackController(IAuthService authService, IFeedbackService feedbackService, IRequestService requestService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
            _requestService = requestService;
        }

        [HttpGet]
        public async Task<IActionResult> PostTaskFeedbackCapture(string j, string r, string f, CancellationToken cancellationToken)
        {
            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return Redirect("/Error/401");
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
            FeedbackRating feedbackRating = (FeedbackRating)Base64Utils.Base64DecodeToInt(f);
            var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);

            if (await _feedbackService.GetFeedbackExists(jobId, requestRole))
            {
                return ShowMessage(FeedbackCaptureMessageViewModel.Messages.FeedbackAlreadyRecorded, job.ReferringGroupID);
            }

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
            var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);
            var result = await _feedbackService.PostRecordFeedback(user, model);

            return ShowMessage(result, job.ReferringGroupID, model.FeedbackRating);
        }

        public IActionResult ShowMessage(FeedbackCaptureMessageViewModel.Messages message, int referringGroupId, FeedbackRating? feedbackRating = null)
        {
            var notification = message switch
            {
                FeedbackCaptureMessageViewModel.Messages.Success => new NotificationModel
                {
                    Type = NotificationType.Success,
                    Title = "Thank you",
                    Subtitle = "Your feedback has been received",
                    Message = $"<p>We'll use your feedback to make HelpMyStreet {(feedbackRating == FeedbackRating.HappyFace ? "even better" : "as good as it can be")}</p>"
                },
                FeedbackCaptureMessageViewModel.Messages.FeedbackAlreadyRecorded or
                FeedbackCaptureMessageViewModel.Messages.IncorrectJobStatus or
                FeedbackCaptureMessageViewModel.Messages.RequestArchived => new NotificationModel
                {
                    Type = NotificationType.Failure_Permanent,
                    Title = "Sorry, that didn't work",
                    Subtitle = "We couldn't record your feedback",
                    Message = "<p>We may already have feedback relating to that request.</p><p>If you'd like to get in touch, please email <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                FeedbackCaptureMessageViewModel.Messages.ServerError => new NotificationModel
                {
                    Type = NotificationType.Failure_Temporary,
                    Title = "Sorry, that didn't work",
                    Subtitle = "We couldn't record your feedback at this time",
                    Message = "<p>This is usually a temporary problem; please press your browser's back button to try again.</p><p>Alternatively, you can email us at <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                _ => throw new ArgumentException($"Unexpected FeedbackCaptureMessageViewModel.Messages value: {message}", nameof(message))
            };

            var vm = new SuccessViewModel()
            {
                //TODO: Don't assume all groups have open request forms
                RequestLink = $"/request-help/{Base64Utils.Base64Encode(referringGroupId)}",
                Notifications = new List<NotificationModel> { notification },
            };

            return View("PostTaskFeedbackCaptureMessage", vm);
        }
    }
}
