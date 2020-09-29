using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICommunicationService _communicationService;

        public FeedbackController(IAuthService authService, IFeedbackRepository feedbackRepository, ICommunicationService communicationService)
        {
            _authService = authService;
            _feedbackRepository = feedbackRepository;
            _communicationService = communicationService;
        }

        public IActionResult PostTaskFeedbackCapture(int jobId, RequestRoles requestRole)
        {
            return View(new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FeedbackCaptureEditModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Sorry, that didn't work." });
            }

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            var jobId = Base64Utils.Base64DecodeToInt(model.EncodedJobId);

            await _feedbackRepository.PostRecordFeedback(jobId, model.RoleSubmittingFeedback, user?.ID, model.FeedbackRating);

            if (!string.IsNullOrEmpty(model.RecipientMessage))
            {
                await _communicationService.SendInterUserMessage(model.RecipientMessage);
            }

            if (!string.IsNullOrEmpty(model.RequestorMessage))
            {
                await _communicationService.SendInterUserMessage(model.RequestorMessage);
            }

            if (!string.IsNullOrEmpty(model.VolunteerMessage))
            {
                await _communicationService.SendInterUserMessage(model.VolunteerMessage);
            }

            if (!string.IsNullOrEmpty(model.GroupMessage))
            {
                await _communicationService.SendInterUserMessage(model.GroupMessage);
            }

            if (!string.IsNullOrEmpty(model.HMSMessage))
            {
                await _communicationService.SendInterUserMessage(model.HMSMessage);
            }

            return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Thanks for your feedback." });
        }

    }
}
