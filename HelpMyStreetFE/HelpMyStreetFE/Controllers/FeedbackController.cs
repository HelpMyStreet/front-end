using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
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

        public FeedbackController(IAuthService authService, IFeedbackRepository feedbackRepository)
        {
            _authService = authService;
            _feedbackRepository = feedbackRepository;
        }

        public IActionResult PostTaskFeedbackCapture(int jobId, RequestRoles requestRole)
        {
            return View(new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FeedbackCaptureEditModel model, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            var jobId = Base64Utils.Base64DecodeToInt(model.EncodedJobId);

            await _feedbackRepository.PostRecordFeedback(jobId, model.RoleSubmittingFeedback, user?.ID, model.FeedbackRating);

            return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Thanks for your feedback." });
        }

    }
}
