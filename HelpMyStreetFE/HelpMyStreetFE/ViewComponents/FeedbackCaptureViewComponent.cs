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
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
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

        public async Task<IViewComponentResult> InvokeAsync(FeedbackCaptureViewComponentParameters parameters, FeedbackCaptureMessageViewModel message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                return View("FeedbackCaptureMessage", message);
            }
            else
            {
                return await InvokeAsync(parameters, cancellationToken);
            }
        }

        private async Task<IViewComponentResult> InvokeAsync(FeedbackCaptureViewComponentParameters parameters, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            int authorisingUserId = parameters.RequestRole == RequestRoles.Volunteer || parameters.RequestRole == RequestRoles.GroupAdmin ? user.ID : -1;

            var jobDetails = await _requestService.GetJobDetailsAsync(parameters.JobId, authorisingUserId, cancellationToken);

            if (jobDetails == null || jobDetails.JobSummary == null)
            {
                throw new Exception($"Attempt to load feedback form for job {parameters.JobId} which could not be found (authorising user id {authorisingUserId})");
            }

            if (jobDetails.JobSummary.JobStatus == JobStatuses.Open || jobDetails.JobSummary.JobStatus == JobStatuses.InProgress)
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.IncorrectJobStatus });
            }

            if (await _feedbackRepository.GetFeedbackExists(parameters.JobId, parameters.RequestRole))
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.FeedbackAlreadyRecorded });
            }

            FeedbackCaptureEditModel viewModel = new FeedbackCaptureEditModel();

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

            return View(parameters.RenderAsPopup ? "FeedbackCapturePopup" : "FeedbackCapture", viewModel);
        }
    }
}
