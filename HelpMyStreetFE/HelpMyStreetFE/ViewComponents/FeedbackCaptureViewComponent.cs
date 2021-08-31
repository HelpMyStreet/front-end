using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Account.Jobs;
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

        public async Task<IViewComponentResult> InvokeAsync(FeedbackCaptureViewComponentParameters parameters, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            int authorisingUserId = parameters.RequestRole == RequestRoles.Volunteer || parameters.RequestRole == RequestRoles.GroupAdmin ? user.ID : -1;
            var jobDetails = await _requestService.GetJobDetailsAsync(parameters.JobId, authorisingUserId, parameters.RequestRole == RequestRoles.GroupAdmin, cancellationToken);

            await EnsureFeedbackCanBeGiven(jobDetails, parameters.RequestRole, user?.ID);

            FeedbackCaptureEditModel viewModel = new FeedbackCaptureEditModel
            {
                RoleSubmittingFeedback = parameters.RequestRole,
                FeedbackRating = parameters.FeedbackRating,

                VolunteerName = jobDetails.CurrentVolunteer?.UserPersonalDetails.DisplayName,
                RecipientName = string.IsNullOrEmpty(jobDetails.RecipientOrganisation) ? jobDetails.Recipient?.FirstName : jobDetails.RecipientOrganisation,
                RequestorName = jobDetails.Requestor?.FirstName,
                
                ShowVolunteerMessage = parameters.RequestRole != RequestRoles.Volunteer && jobDetails.CurrentVolunteer != null,
                ShowRecipientMessage = parameters.RequestRole != RequestRoles.Recipient && !string.IsNullOrEmpty(jobDetails.Recipient?.EmailAddress),
                ShowRequestorMessage = parameters.RequestRole != RequestRoles.Requestor && !string.IsNullOrEmpty(jobDetails.Requestor?.EmailAddress) && jobDetails.RequestorType != RequestorType.Myself,
                ShowHMSMessage = true
            };
            
            if (jobDetails.ReferringGroupID != (int)Groups.Generic)
            {
                var groupDetails = await _groupService.GetGroupById(jobDetails.ReferringGroupID, cancellationToken);
                viewModel.GroupName = groupDetails.GroupName;
                viewModel.ShowGroupMessage = (parameters.RequestRole != RequestRoles.GroupAdmin);

                // TODO: Replace with RequestorDefinedByGroup
                if (jobDetails.ReferringGroupID == (int)Groups.AgeUKWirral)
                {
                    viewModel.ShowRequestorMessage = false;
                }
            }
            else
            {
                viewModel.ShowGroupMessage = false;
            }

            return View(parameters.RenderAsPopup ? "FeedbackCapturePopup" : "FeedbackCapture", viewModel);
        }

        private async Task EnsureFeedbackCanBeGiven(JobSummary jobSummary, RequestRoles requestRole, int? userId)
        {
            if (jobSummary.JobStatus.Incomplete())
            {
                throw new Exception($"Attempt to load feedback form for job {jobSummary.JobID}, but it is {jobSummary.JobStatus}");
            }

            if (await _feedbackRepository.GetFeedbackExists(jobSummary.JobID, requestRole, userId))
            {
                throw new Exception($"Attempt to load feedback form for job {jobSummary.JobID}, but feedback already exists for role {requestRole} / user {userId}");
            }

            if (jobSummary.Archive == true)
            {
                throw new Exception($"Attempt to load feedback form for achived job {jobSummary.JobID}");
            }
        }
    }
}
