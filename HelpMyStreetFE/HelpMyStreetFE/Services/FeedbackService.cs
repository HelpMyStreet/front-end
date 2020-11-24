using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Repositories;

namespace HelpMyStreetFE.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICommunicationService _communicationService;
        private readonly IRequestHelpRepository _requestHelpRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository, ICommunicationService communicationService, IRequestHelpRepository requestHelpRepository)
        {
            _feedbackRepository = feedbackRepository;
            _communicationService = communicationService;
            _requestHelpRepository = requestHelpRepository;
        }

        public async Task<bool> GetFeedbackExists(int jobId, RequestRoles requestRole, int? userId)
        {
            return await _feedbackRepository.GetFeedbackExists(jobId, requestRole, userId);
        }

        public async Task<Result> PostRecordFeedback(User user, CapturedFeedback feedback)
        {
            var job = await _requestHelpRepository.GetJobDetailsAsync(feedback.JobId, -1);

            if (job == null || job.JobSummary == null)
            {
                throw new Exception($"Attempt to submit feedback for job {feedback.JobId} which could not be found");
            }

            if (job.JobSummary.JobStatus == JobStatuses.Open || job.JobSummary.JobStatus == JobStatuses.InProgress)
            {
                return Result.Failure_IncorrectJobStatus;
            }



            bool success = await _feedbackRepository.PostRecordFeedback(feedback.JobId, feedback.RoleSubmittingFeedback, user?.ID, feedback.FeedbackRating);

            if (!success)
            {
                if (await _feedbackRepository.GetFeedbackExists(feedback.JobId, feedback.RoleSubmittingFeedback, user?.ID))
                {
                    return Result.Failure_FeedbackAlreadyRecorded;
                }
                else
                {
                    return Result.Failure_ServerError;
                }
            }

            MessageParticipant from = GetFromBlock(user, feedback.RoleSubmittingFeedback, job);

            if (!string.IsNullOrEmpty(feedback.RecipientMessage))
            {
                var to = GetToBlock(job, RequestRoles.Recipient);
                success &= await _communicationService.SendInterUserMessage(from, to, feedback.RecipientMessage, feedback.JobId);
            }

            if (!string.IsNullOrEmpty(feedback.RequestorMessage))
            {
                var to = GetToBlock(job, RequestRoles.Requestor);
                success &= await _communicationService.SendInterUserMessage(from, to, feedback.RequestorMessage, feedback.JobId);
            }

            if (!string.IsNullOrEmpty(feedback.VolunteerMessage))
            {
                var to = GetToBlock(job, RequestRoles.Volunteer);
                success &= await _communicationService.SendInterUserMessage(from, to, feedback.VolunteerMessage, feedback.JobId);
            }

            if (!string.IsNullOrEmpty(feedback.GroupMessage))
            {
                var to = GetToBlock(job, RequestRoles.GroupAdmin);
                success &= await _communicationService.SendInterUserMessage(from, to, feedback.GroupMessage, feedback.JobId);
            }

            if (!string.IsNullOrEmpty(feedback.HMSMessage))
            {
                var to = new MessageParticipant()
                {
                    GroupRoleType = new GroupRoleType()
                    {
                        GroupId = (int)HelpMyStreet.Utils.Enums.Groups.Generic,
                        GroupRoles = GroupRoles.Owner
                    },
                    RequestRoleType = new RequestRoleType() { RequestRole = RequestRoles.GroupAdmin }
                };
                success &= await _communicationService.SendInterUserMessage(from, to, feedback.HMSMessage, feedback.JobId);
            }

            return success ? Result.Success : Result.Failure_ServerError;
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
                        RequestRoles.Recipient => string.IsNullOrEmpty(job.JobSummary.RecipientOrganisation) ? job.Recipient.FirstName : job.JobSummary.RecipientOrganisation,
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
                    to.EmailDetails = new EmailDetails()
                    {
                        DisplayName = string.IsNullOrEmpty(job.JobSummary.RecipientOrganisation) ? job.Recipient.FirstName : job.JobSummary.RecipientOrganisation,
                        EmailAddress = job.Recipient.EmailAddress
                    };
                    break;
                case RequestRoles.Requestor:
                    to.EmailDetails = new EmailDetails()
                    {
                        DisplayName = job.Requestor.FirstName,
                        EmailAddress = job.Requestor.EmailAddress
                    };
                    break;
                case RequestRoles.Volunteer:
                    to.UserId = job.JobSummary.VolunteerUserID;
                    break;
                case RequestRoles.GroupAdmin:
                    to.GroupRoleType = new GroupRoleType()
                    {
                        GroupId = job.JobSummary.ReferringGroupID,
                        GroupRoles = GroupRoles.Owner
                    };
                    break;
            }

            return to;
        }
    }
}
