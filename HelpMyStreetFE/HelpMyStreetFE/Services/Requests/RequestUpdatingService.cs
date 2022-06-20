using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestUpdatingService : IRequestUpdatingService
    {
        private readonly IRequestService _requestService;
        private readonly IRequestListCachingService _requestListCachingService;
        private readonly IRequestCachingService _requestCachingService;
        private readonly IJobCachingService _jobCachingService;
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IRequestHelpBuilder _requestHelpBuilder;

        public RequestUpdatingService(
            IRequestService requestService,
            IRequestListCachingService requestListCachingService,
            IRequestCachingService requestCachingService,
            IJobCachingService jobCachingService,
            IRequestHelpRepository requestHelpRepository,
            ILogger<RequestService> logger,
            IRequestHelpBuilder requestHelpBuilder)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _requestListCachingService = requestListCachingService ?? throw new ArgumentNullException(nameof(requestListCachingService));
            _requestCachingService = requestCachingService ?? throw new ArgumentNullException(nameof(requestCachingService));
            _jobCachingService = jobCachingService ?? throw new ArgumentNullException(nameof(jobCachingService));
            _requestHelpRepository = requestHelpRepository ?? throw new ArgumentNullException(nameof(requestHelpRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestHelpBuilder = requestHelpBuilder ?? throw new ArgumentNullException(nameof(requestHelpBuilder));
        }

        public async Task<Fulfillable> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, string language, User user, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Logging Request");

            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();
            var selectedFrequency = requestStage.Frequencies.Where(x => x.IsSelected).FirstOrDefault();

            int numberOfOccurrences = 1;
            if (requestStage.Occurrences.HasValue && !selectedFrequency.Frequency.Equals(Frequency.Once))
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

            var request = new PostRequestForHelpRequest
            {
                HelpRequestDetails = new List<HelpRequestDetail>()
                {
                   new HelpRequestDetail()
                   {
                        HelpRequest = new HelpRequest
                        {
                            Guid = requestStage.RequestGuid,
                            AcceptedTerms = requestStage.AgreeToPrivacyAndTerms,
                            ConsentForContact = requestStage.AgreeToPrivacyAndTerms,
                            OrganisationName = detailStage?.Organisation ?? "",
                            RequestorType = detailStage?.Type ?? RequestorType.Organisation,
                            ReadPrivacyNotice = requestStage.AgreeToPrivacyAndTerms,
                            CreatedByUserId = user?.ID ?? 0,
                            Recipient = recipient,
                            Requestor = requestor,
                            ReferringGroupId = referringGroupID,
                            Source = source,
                            Language = language
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
                   }
                }
            };

            var json = JsonConvert.SerializeObject(request);

            var response = await _requestHelpRepository.PostRequestForHelpAsync(request);
            if (response != null)
            {
                _ = _requestCachingService.RefreshCacheAsync(response.RequestIDs, cancellationToken);
                _ = _requestListCachingService.RefreshGroupRequestsCacheAsync(referringGroupID, cancellationToken);

                if (user != null)
                {
                    _ = _requestListCachingService.RefreshUserOpenJobsCacheAsync(user, cancellationToken);
                }
            }

            return response.Fulfillable;
        }

        public async Task<UpdateJobOutcome?> UpdateJobQuestion (int jobId, int questionId, string answer, int authorisedByUserId, CancellationToken cancellationToken)
        {
            var outcome = await _requestHelpRepository.PutUpdateJobQuestion(jobId, questionId, answer, authorisedByUserId);

            if (outcome == UpdateJobOutcome.Success || outcome == UpdateJobOutcome.AlreadyInThisState)
            {
                _ = _jobCachingService.RefreshCacheAsync(jobId, cancellationToken);
            }

            return outcome;
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
                _ = _requestCachingService.RefreshCacheAsync(requestId, cancellationToken);
            }

            return outcome;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken)
        {
            UpdateJobStatusOutcome? outcome = status switch
            {
                JobStatuses.AppliedFor => await UpdateJobStatusToAppliedForAsync(jobID, createdByUserId, volunteerUserId.Value, cancellationToken),
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
                _ = _jobCachingService.RefreshCacheAsync(jobID, cancellationToken);

                if (status == JobStatuses.Accepted || status == JobStatuses.InProgress)
                {
                    _ = _requestListCachingService.RefreshUserRequestsCacheAsync(volunteerUserId.Value, cancellationToken);
                }
            }

            return outcome;
        }

        private async Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken)
        {
            var job = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);

            UpdateJobStatusOutcome? outcome = job.RequestType switch
            {
                RequestType.Shift => await _requestHelpRepository.PutUpdateShiftStatusToAccepted(job.RequestID, job.SupportActivity, createdByUserId, volunteerUserId),
                RequestType.Task => await _requestHelpRepository.UpdateJobStatusToInProgressAsync(jobId, createdByUserId, volunteerUserId),
                _ => throw new ArgumentException(message: $"Invalid RequestType value: {job.RequestType}", paramName: nameof(job.RequestType)),
            };

            return outcome;
        }

        private async Task<UpdateJobStatusOutcome?> UpdateJobStatusToAppliedForAsync(int jobId, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken)
        {
            var job = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);

            UpdateJobStatusOutcome? outcome = job.RequestType switch
            {
                RequestType.Task => await _requestHelpRepository.UpdateJobStatusToAppliedForAsync(jobId, createdByUserId, volunteerUserId),
                _ => throw new ArgumentException(message: $"Invalid RequestType value: {job.RequestType}", paramName: nameof(job.RequestType)),
            };

            return outcome;
        }

        private string GetAnswerToQuestion(RequestHelpQuestion q)
        {
            return q.InputType == QuestionType.Radio ? q.AdditionalData.Where(a => a.Key == q.Model).FirstOrDefault()?.Value ?? "" : q.Model;
        }
    }
}
