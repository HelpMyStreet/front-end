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

namespace HelpMyStreetFE.ViewComponents
{
    public class FeedbackCaptureViewComponent : ViewComponent
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IGroupService _groupService;

        public FeedbackCaptureViewComponent(IFeedbackRepository feedbackRepository, IRequestService requestService, IAuthService authService, IGroupService groupService)
        {
            _feedbackRepository = feedbackRepository;
            _requestService = requestService;
            _authService = authService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync(FeedbackCaptureViewComponentParameters parameters, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            // user may be null for Requestor/Recipient feedback

            var jobDetails = await _requestService.GetJobDetailsAsync(parameters.JobId, user?.ID ?? -1, cancellationToken);

            if (jobDetails == null || jobDetails.JobSummary == null)
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Sorry, we can't find that job." });
            }

            if (jobDetails.JobSummary.JobStatus == JobStatuses.Open || jobDetails.JobSummary.JobStatus == JobStatuses.InProgress)
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Sorry, feedback can only be submitted for jobs which are completed." });
            }

            if (await _feedbackRepository.GetFeedbackExists(parameters.JobId, parameters.RequestRole))
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = "Sorry, feedback for this job has already been submitted." });
            }

            FeedbackCaptureEditModel viewModel = new FeedbackCaptureEditModel();

            viewModel.EncodedJobId = Base64Utils.Base64Encode(parameters.JobId);
            viewModel.RoleSubmittingFeedback = parameters.RequestRole;

            viewModel.VolunteerName = jobDetails.CurrentVolunteer?.UserPersonalDetails.DisplayName;
            viewModel.RecipientName = jobDetails.Recipient.FirstName;
            viewModel.RequestorName = jobDetails.Requestor.FirstName;

            viewModel.ShowVolunteerMessage = parameters.RequestRole != RequestRoles.Volunteer && jobDetails.CurrentVolunteer != null;
            viewModel.ShowRecipientMessage = parameters.RequestRole != RequestRoles.Recipient;
            viewModel.ShowRequestorMessage = parameters.RequestRole != RequestRoles.Requestor && jobDetails.JobSummary.RequestorType != RequestorType.Myself;
            viewModel.ShowHMSMessage = true;

            if (jobDetails.JobSummary.ReferringGroupID != -1)
            {
                var groupDetails = await _groupService.GetGroupById(jobDetails.JobSummary.ReferringGroupID, cancellationToken);
                viewModel.GroupName = groupDetails.GroupName;
                viewModel.ShowGroupMessage = (parameters.RequestRole != RequestRoles.GroupAdmin);
            }
            else
            {
                viewModel.ShowGroupMessage = false;
            }

            return View("FeedbackCapture", viewModel);
        }
    }
}
