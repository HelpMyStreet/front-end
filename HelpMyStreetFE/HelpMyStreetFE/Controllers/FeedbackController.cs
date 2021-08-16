using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;
        private readonly IJobCachingService _jobCachingService;

        public FeedbackController(IAuthService authService, IFeedbackService feedbackService, IJobCachingService jobCachingService)
        {
            _authService = authService;
            _feedbackService = feedbackService;
            _jobCachingService = jobCachingService;
        }

        [HttpGet]
        public async Task<IActionResult> PostTaskFeedbackCapture(string j, string r, string f, CancellationToken cancellationToken)
        {
            if (!_authService.GetUrlIsSessionAuthorised())
            {
                return Redirect("/Error/401");
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
            FeedbackRating feedbackRating = string.IsNullOrEmpty(f) ? 0 : (FeedbackRating)Base64Utils.Base64DecodeToInt(f);
            var job = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);

            if (job.JobStatus.Incomplete())
            {
                return ShowMessage(Result.Failure_IncorrectJobStatus, job.ReferringGroupID);
            }

            if (await _feedbackService.GetFeedbackExists(jobId, requestRole, user?.ID))
            {
                return ShowMessage(Result.Failure_FeedbackAlreadyRecorded, job.ReferringGroupID);
            }

            return View(new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole, FeedbackRating = feedbackRating });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostTaskFeedbackCapture(string j, string r, FeedbackCaptureEditModel model, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            if (!_authService.GetUrlIsSessionAuthorised())
            {
                return Redirect("/Error/401");
            }
            if (!ModelState.IsValid)
            {
                throw new Exception($"Invalid model state in PostTaskFeedbackCapture for job {jobId}");
            }

            model.JobId = jobId;
            model.RoleSubmittingFeedback = requestRole;

            var user = await _authService.GetCurrentUser(cancellationToken);
            var job = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);
            var result = await _feedbackService.PostRecordFeedback(user, model);

            return ShowMessage(result, job.ReferringGroupID, model);
        }

        public IActionResult ShowMessage(Result result, int referringGroupId, CapturedFeedback capturedFeedback = null)
        {
            var notification = result switch
            {
                Result.Success => new NotificationModel
                {
                    Type = NotificationType.Success,
                    Title = "Thank you",
                    Subtitle = "Your comments have been submitted",
                    Message = $"<p>We’ll use your feedback to make HelpMyStreet {(capturedFeedback.FeedbackRating == FeedbackRating.HappyFace ? "even better" : "as good as it can be")}.</p>{(capturedFeedback.IncludesMessage ? "<p>We’ll pass your messages on to the people involved with this request.</p>" : "")}"
                },
                Result.Failure_IncorrectJobStatus => new NotificationModel
                {
                    Type = NotificationType.Failure_Permanent,
                    Title = "Sorry, that didn't work",
                    Subtitle = "We couldn’t record your feedback",
                    Message = "<p>The request is not currently marked as complete in our system.</p><p>If you’d like to get in touch, please email <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                Result.Failure_FeedbackAlreadyRecorded => new NotificationModel
                {
                    Type = NotificationType.Failure_Permanent,
                    Title = "Sorry, that didn’t work",
                    Subtitle = "We couldn’t record your feedback",
                    Message = "<p>We may already have feedback relating to that request.</p><p>If you’d like to get in touch, please email <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                Result.Failure_RequestArchived => new NotificationModel
                {
                    Type = NotificationType.Failure_Permanent,
                    Title = "Sorry, that didn't work",
                    Subtitle = "We couldn’t record your feedback",
                    Message = "<p>That request may have been too long ago.</p><p>If you’d like to get in touch, please email <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                Result.Failure_ServerError => new NotificationModel
                {
                    Type = NotificationType.Failure_Temporary,
                    Title = "Sorry, that didn’t work",
                    Subtitle = "We couldn’t record your feedback at this time",
                    Message = "<p>This is usually a temporary problem; please press your browser’s back button to try again.</p><p>Alternatively, you can email us at <a href='mailto:feedback@helpmystreet.org'>feedback@helpmystreet.org</a>.</p>"
                },
                _ => throw new ArgumentException($"Unexpected Result value: {result}", nameof(result))
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
