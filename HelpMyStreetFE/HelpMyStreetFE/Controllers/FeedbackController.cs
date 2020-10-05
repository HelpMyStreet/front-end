using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICommunicationService _communicationService;
        private readonly IRequestHelpRepository _requestHelpRepository;

        public FeedbackController(IAuthService authService, IFeedbackRepository feedbackRepository, ICommunicationService communicationService, IRequestHelpRepository requestHelpRepository)
        {
            _authService = authService;
            _feedbackRepository = feedbackRepository;
            _communicationService = communicationService;
            _requestHelpRepository = requestHelpRepository;
        }

        [HttpGet]
        public IActionResult PostTaskFeedbackCapture(string j, string r)
        {
            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return Redirect("/Error/401");
            }

            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            return View(new FeedbackCaptureViewComponentParameters() { JobId = jobId, RequestRole = requestRole });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostTaskFeedbackCapture(string j, string r, FeedbackCaptureEditModel model, CancellationToken cancellationToken)
        {
            if (!_authService.GetUrlIsSessionAuthorised(HttpContext))
            {
                return Redirect("/Error/401");
            }

            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
            int jobId = Base64Utils.Base64DecodeToInt(j);
            var job = await _requestHelpRepository.GetJobDetailsAsync(jobId, -1);

            if (!ModelState.IsValid)
            {
                throw new Exception($"Invalid model state in PostTaskFeedbackCapture for job {jobId}");
            }

            if (job == null || job.JobSummary == null)
            {
                throw new Exception($"Attempt to submit feedback for job {jobId} which could not be found");
            }

            if (job.JobSummary.JobStatus == JobStatuses.Open || job.JobSummary.JobStatus == JobStatuses.InProgress)
            {
                return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.IncorrectJobStatus });
            }


            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            bool postRecordFeedbackSuccess = await _feedbackRepository.PostRecordFeedback(jobId, requestRole, user?.ID, model.FeedbackRating);

            if (!postRecordFeedbackSuccess)
            {
                if (await _feedbackRepository.GetFeedbackExists(jobId, requestRole))
                {
                    return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.FeedbackAlreadyRecorded});
                }
                else
                {
                    return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.ServerError });
                }
            }

            MessageParticipant from = GetFromBlock(user, requestRole, job);

            if (!string.IsNullOrEmpty(model.RecipientMessage))
            {
                var to = GetToBlock(job, RequestRoles.Recipient);
                await _communicationService.SendInterUserMessage(from, to, model.RecipientMessage, jobId);
            }

            if (!string.IsNullOrEmpty(model.RequestorMessage))
            {
                var to = GetToBlock(job, RequestRoles.Requestor);
                await _communicationService.SendInterUserMessage(from, to, model.RequestorMessage, jobId);
            }

            if (!string.IsNullOrEmpty(model.VolunteerMessage))
            {
                var to = GetToBlock(job, RequestRoles.Volunteer);
                await _communicationService.SendInterUserMessage(from, to, model.VolunteerMessage, jobId);
            }

            if (!string.IsNullOrEmpty(model.GroupMessage))
            {
                var to = GetToBlock(job, RequestRoles.GroupAdmin);
                await _communicationService.SendInterUserMessage(from, to, model.GroupMessage, jobId);
            }

            if (!string.IsNullOrEmpty(model.HMSMessage))
            {
                var to = new MessageParticipant()
                {
                    GroupRoleType = new GroupRoleType()
                    {
                        GroupId = (int)Groups.Generic,
                        GroupRoles = GroupRoles.Owner
                    },
                    RequestRoleType = new RequestRoleType() { RequestRole = RequestRoles.GroupAdmin }
                };
                await _communicationService.SendInterUserMessage(from, to, model.HMSMessage, jobId);
            }

            return View("FeedbackCaptureMessage", new FeedbackCaptureMessageViewModel() { Message = FeedbackCaptureMessageViewModel.Messages.Success });
        }

        private MessageParticipant GetFromBlock(User user, RequestRoles requestRole, GetJobDetailsResponse job)
        {
            MessageParticipant from = new MessageParticipant();

            if (requestRole == RequestRoles.GroupAdmin)
            {
                from.GroupRoleType = new GroupRoleType()
                {
                    GroupId = job.JobSummary.ReferringGroupID,
                    GroupRoles = GroupRoles.Owner
                };
            }
            else if (user != null)
            {
                from.UserId = user.ID;
            }
            else
            {
                from.EmailDetails = new EmailDetails()
                {
                    DisplayName = requestRole switch
                    {
                        RequestRoles.Recipient => job.Recipient.FirstName,
                        RequestRoles.Requestor => job.Requestor.FirstName,
                        _ => throw new ArgumentException(message: $"Unexpected RequestRoles value: {requestRole}", paramName: nameof(requestRole))
                    }
                };
            }

            from.RequestRoleType = new RequestRoleType() { RequestRole = requestRole };

            return from;
        }

        private MessageParticipant GetToBlock(GetJobDetailsResponse job, RequestRoles requestRole)
        {
            MessageParticipant to = new MessageParticipant() { RequestRoleType = new RequestRoleType() { RequestRole = requestRole } };

            switch (requestRole)
            {
                case RequestRoles.Recipient:
                    to.EmailDetails = new EmailDetails() { DisplayName = job.Recipient.FirstName, EmailAddress = job.Recipient.EmailAddress };
                    break;
                case RequestRoles.Requestor:
                    to.EmailDetails = new EmailDetails() { DisplayName = job.Requestor.FirstName, EmailAddress = job.Requestor.EmailAddress };
                    break;
                case RequestRoles.Volunteer:
                    to.UserId = job.JobSummary.VolunteerUserID;
                    break;
                case RequestRoles.GroupAdmin:
                    to.GroupRoleType = new GroupRoleType() { GroupId = job.JobSummary.ReferringGroupID, GroupRoles = GroupRoles.Owner };
                    break;
            }

            return to;
        }
    }
}
