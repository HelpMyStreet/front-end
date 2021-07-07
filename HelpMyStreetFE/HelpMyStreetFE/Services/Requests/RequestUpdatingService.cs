using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestUpdatingService : IRequestUpdatingService
    {
        private readonly IRequestService _requestService;
        private readonly IRequestCachingService _requestCachingService;
        private readonly IJobCachingService _jobCachingService;
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        //private readonly IGroupService _groupService;
        //private readonly IUserService _userService;
        //private readonly IMemDistCache<IEnumerable<JobSummary>> _memDistCache;
        //private readonly IMemDistCache<IEnumerable<ShiftJob>> _memDistCache_ShiftJobs;
        //private readonly IMemDistCache<IEnumerable<RequestSummary>> _memDistCache_RequestSummaries;
        //private readonly IOptions<RequestSettings> _requestSettings;
        //private readonly IGroupMemberService _groupMemberService;
        //private readonly IAddressService _addressService;


        //private const string CACHE_KEY_PREFIX = "request-service-jobs";

        public RequestUpdatingService(
            IRequestService requestService,
            IRequestHelpRepository requestHelpRepository, 
            ILogger<RequestService> logger, 
            IRequestHelpBuilder requestHelpBuilder, 
            //IGroupService groupService, 
            //IUserService userService, 
            //IMemDistCache<IEnumerable<JobSummary>> memDistCache, 
            //IGroupMemberService groupMemberService, 
            //IAddressService addressService, 
            //IMemDistCache<IEnumerable<ShiftJob>> memDistCache_ShiftJobs, 
            //IMemDistCache<IEnumerable<RequestSummary>> memDistCache_RequestSummaries, 
            IRequestCachingService requestCachingService, 
            IJobCachingService jobCachingService)
        {
            _requestService = requestService;
            _requestCachingService = requestCachingService;
            _jobCachingService = jobCachingService;
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestHelpBuilder = requestHelpBuilder;
            //_groupService = groupService;
            //_userService = userService;
            //_memDistCache = memDistCache;
            //_requestSettings = requestSettings;
            //_groupMemberService = groupMemberService;
            //_addressService = addressService;
            //_memDistCache_ShiftJobs = memDistCache_ShiftJobs;
            //_memDistCache_RequestSummaries = memDistCache_RequestSummaries;
        }

        public async Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Logging Request");

            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();
            var selectedFrequency = requestStage.Frequencies.Where(x => x.IsSelected).FirstOrDefault();

            int numberOfOccurrences = 1;
            if (requestStage.Occurrences.HasValue)
            {
                numberOfOccurrences = requestStage.Occurrences.Value;
            }

            bool heathCritical = false;
            var healthCriticalQuestion = requestStage.Questions.Questions.Where(a => a.ID == (int)Questions.IsHealthCritical).FirstOrDefault();
            if (healthCriticalQuestion != null && healthCriticalQuestion.Model == "true") { heathCritical = true; }

            RequestPersonalDetails recipient = null;
            RequestPersonalDetails requestor = null;
            IEnumerable<RequestHelpQuestion> questions = requestStage.Questions.Questions;

            if (detailStage != null)
            {
                recipient = _requestHelpBuilder.MapRecipient(detailStage);
                if (detailStage.ShowRequestorFields)
                {
                    requestor = detailStage.Type == RequestorType.Myself ? recipient : _requestHelpBuilder.MapRequestor(detailStage);
                }
                questions = questions.Union(detailStage.Questions.Questions);
            }

            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest
                {
                    Guid = requestStage.RequestGuid,
                    AcceptedTerms = requestStage.AgreeToPrivacyAndTerms,
                    ConsentForContact = requestStage.AgreeToPrivacyAndTerms,
                    OrganisationName = detailStage?.Organisation ?? "",
                    RequestorType = detailStage?.Type ?? RequestorType.Organisation,
                    ReadPrivacyNotice = requestStage.AgreeToPrivacyAndTerms,
                    CreatedByUserId = userId,
                    Recipient = recipient,
                    Requestor = requestor,
                    ReferringGroupId = referringGroupID,
                    Source = source
                },
                NewJobsRequest = new NewJobsRequest
                {
                    Jobs = new List<Job>
                    {
                        new Job
                        {
                            DueDateType = selectedTime.DueDateType,
                            StartDate = selectedTime.StartTime.ToUTCFromUKTime(),
                            EndDate = selectedTime.EndTime.HasValue ? selectedTime.EndTime.Value.ToUTCFromUKTime() : (DateTime?)null,
                            NotBeforeDate = selectedTime.NotBeforeTime.ToUTCFromUKTime(),
                            RepeatFrequency = selectedFrequency.Frequency,
                            NumberOfRepeats = numberOfOccurrences,
                            HealthCritical = heathCritical,
                            SupportActivity = selectedTask.SupportActivity,
                            Questions = questions.Where(x => x.InputType != QuestionType.LabelOnly).Select(x => new Question {
                                Id = x.ID,
                                Answer = GetAnswerToQuestion(x),
                                Name = x.Label,
                                Required = x.Required,
                                AddtitonalData = x.AdditionalData,
                                Type  = x.InputType}).ToList()
                        }
                    }
                }
            };


            var response = await _requestHelpRepository.PostNewRequestForHelpAsync(request);
            if (response != null && userId != 0)
            {
                _requestService.TriggerCacheRefresh(userId, cancellationToken);
                _requestCachingService.TriggerRequestCacheRefresh(response.RequestID, cancellationToken);
            }

            return response;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateRequestStatusAsync(int requestId, JobStatuses status, int createdByUserId, CancellationToken cancellationToken)
        {
            UpdateJobStatusOutcome? outcome = status switch
            {
                JobStatuses.Done => await _requestHelpRepository.PutUpdateRequestStatusToDone(requestId, createdByUserId),
                JobStatuses.Cancelled => await _requestHelpRepository.PutUpdateRequestStatusToCancelled(requestId, createdByUserId),
                _ => throw new ArgumentException(message: $"Invalid JobStatuses value for Request: {status}", paramName: nameof(status)),
            };

            if (outcome == UpdateJobStatusOutcome.Success || outcome == UpdateJobStatusOutcome.AlreadyInThisStatus)
            {
                _requestService.TriggerCacheRefresh(createdByUserId, cancellationToken);
                _requestCachingService.TriggerRequestCacheRefresh(requestId, cancellationToken);
            }

            return outcome;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken)
        {
            UpdateJobStatusOutcome? outcome = status switch
            {
                JobStatuses.Accepted => await _requestHelpRepository.UpdateJobStatusToAcceptedAsync(jobID, createdByUserId, volunteerUserId.Value),
                JobStatuses.InProgress => await UpdateJobStatusToInProgressAsync(jobID, createdByUserId, volunteerUserId.Value, cancellationToken),
                JobStatuses.Done => await _requestHelpRepository.UpdateJobStatusToDoneAsync(jobID, createdByUserId),
                JobStatuses.Cancelled => await _requestHelpRepository.UpdateJobStatusToCancelledAsync(jobID, createdByUserId),
                JobStatuses.Open => await _requestHelpRepository.UpdateJobStatusToOpenAsync(jobID, createdByUserId),
                JobStatuses.New => await _requestHelpRepository.UpdateJobStatusToNewAsync(jobID, createdByUserId),
                _ => throw new ArgumentException(message: $"Invalid JobStatuses value: {status}", paramName: nameof(status)),
            };

            if (outcome == UpdateJobStatusOutcome.Success || outcome == UpdateJobStatusOutcome.AlreadyInThisStatus)
            {
                _requestService.TriggerCacheRefresh(createdByUserId, cancellationToken);
                _ = _jobCachingService.TriggerCacheRefresh(jobID, cancellationToken);
            }

            return outcome;
        }

        private async Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken)
        {
            var job = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);

            UpdateJobStatusOutcome? outcome = job.RequestType switch
            {
                RequestType.Shift => await _requestHelpRepository.PutUpdateShiftStatusToAccepted(job.RequestID, job.SupportActivity, createdByUserId, volunteerUserId),
                RequestType.Task => await _requestHelpRepository.UpdateJobStatusToInProgressAsync(jobId, createdByUserId, volunteerUserId),
                _ => throw new ArgumentException(message: $"Invalid RequestType value: {job.RequestType}", paramName: nameof(job.RequestType)),
            };

            _requestService.TriggerCacheRefresh(createdByUserId, cancellationToken);
            _ = _jobCachingService.TriggerCacheRefresh(jobId, cancellationToken);

            return outcome;
        }

        private string GetAnswerToQuestion(RequestHelpQuestion q)
        {
            return q.InputType == QuestionType.Radio ? q.AdditionalData.Where(a => a.Key == q.Model).FirstOrDefault()?.Value ?? "" : q.Model;
        }
    }
}
