using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using System.Linq;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreetFE.Models.Account;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Contracts.RequestService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Enums.RequestHelp;

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IOptions<RequestSettings> _requestSettings;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IOptions<RequestSettings> requestSettings, IRequestHelpBuilder requestHelpBuilder)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestSettings = requestSettings;
            _requestHelpBuilder = requestHelpBuilder;
    }

        public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, RequestHelpSource source, int userId, HttpContext ctx)
        {
            _logger.LogInformation($"Logging Request");
            var recipient = _requestHelpBuilder.MapRecipient(detailStage);
            var requestor = detailStage.Type == RequestorType.OnBehalf || detailStage.Type == RequestorType.Organisation ? _requestHelpBuilder.MapRequestor(detailStage) : recipient;
            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();

            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest
                {
                    AcceptedTerms = requestStage.AgreeToTerms,
                    OtherDetails = detailStage.OtherDetails,
                    ConsentForContact = requestStage.AgreeToTerms,
                    SpecialCommunicationNeeds = detailStage.CommunicationNeeds,
                    OrganisationName = detailStage.Organisation ?? "",
                    RequestorType = detailStage.Type,
                    ForRequestor = detailStage.Type == RequestorType.Myself ? true : false,
                    ReadPrivacyNotice = requestStage.AgreeToPrivacy,
                    CreatedByUserId = userId,
                    Recipient = recipient,
                    Requestor = requestor,
                    VolunteerUserId = _requestHelpBuilder.GetVolunteerUserID(requestStage, detailStage.Type, source, userId),
                },
                NewJobsRequest = new NewJobsRequest
                {
                    Jobs = new List<Job>
                    {
                        new Job
                        {
                            DueDays = selectedTime.Days,
                            Details = "",
                            HealthCritical = requestStage.IsHealthCritical.HasValue ? requestStage.IsHealthCritical.Value : false,
                            SupportActivity = selectedTask.SupportActivity,
                            Questions = selectedTask.Questions.Select(x => new Question {
                                Id = x.ID,
                                Answer = x.InputType == QuestionType.Radio ? x.AdditionalData.Where(a => a.Key == x.Model).FirstOrDefault()?.Value ?? "" : x.Model,
                                Name = x.Label,
                                Required = x.Required,
                                AddtitonalData = x.AdditionalData,
                                Type  = x.InputType}).ToList()
                        }
                    }
                }
            };


            var response = await _requestHelpRepository.PostNewRequestForHelpAsync(request);
            if (response.HasContent & response.IsSuccessful)
                TriggerCacheRefresh(ctx);


            return response;
        }
        public async Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, User user, HttpContext ctx)
        {
            var jobs = ctx.Session.GetObjectFromJson<OpenJobsViewModel>("openJobs");
            DateTime lastUpdated;
            DateTime.TryParse(ctx.Session.GetString("openJobsLastUpdated"), out lastUpdated);
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)
            {
                var all = await _requestHelpRepository.GetJobsByFilterAsync(user.PostalCode, distanceInMiles);


                // if they dont have the community connector support activity, let remove any open requests in there.
                if (!user.SupportActivities.Contains(SupportActivities.CommunityConnector))
                {
                    all = all.ToList().Where(x => x.SupportActivity != SupportActivities.CommunityConnector);
                };

                var (criteriaJobs, otherJobs) = all.Split(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles < user.SupportRadiusMiles);

                jobs = new OpenJobsViewModel
                {
                    CriteriaJobs = criteriaJobs.OrderOpenJobsForDisplay(),
                    OtherJobs = otherJobs.OrderOpenJobsForDisplay()
                };
                
                ctx.Session.SetObjectAsJson("openJobs", jobs);
                ctx.Session.SetString("openJobsLastUpdated", DateTime.Now.ToString());
            }

            


            return jobs;
        }

        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, HttpContext ctx)
        {
            var jobs = ctx.Session.GetObjectFromJson<IEnumerable<JobSummary>>("acceptedJobs");
            DateTime lastUpdated;
            DateTime.TryParse(ctx.Session.GetString("acceptedJobsLastUpdated"), out lastUpdated);
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)
            {
                jobs = (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId)).OrderOpenJobsForDisplay();
                ctx.Session.SetObjectAsJson("acceptedJobs", jobs);
                ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.ToString());
            }

            return jobs;
        }


        public async Task<IDictionary<int, RequestContactInformation>> GetContactInformationForRequests(IEnumerable<int> ids)
        {
            List<GetJobDetailsResponse> details = new List<GetJobDetailsResponse>();

            foreach (var id in ids)
            {
                details.Add(await _requestHelpRepository.GetJobDetailsAsync(id));
            }

            return details.Aggregate(new Dictionary<int, RequestContactInformation>(), (acc, cur) =>
            {
                acc[cur.JobID] = new RequestContactInformation
                {
                    RequestorType = cur.RequestorType,
                    JobID = cur.JobID,
                    Recipient = cur.Recipient,
                    Requestor = cur.Requestor
                };

                return acc;
            });
        }

        public async Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId)
        {
            return await _requestHelpRepository.GetJobDetailsAsync(jobId);
        }
        public async Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
            {
                JobID = jobID,
                CreatedByUserID = createdByUserId
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }
        public async Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }
        public async Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, HttpContext ctx)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToInProgressAsync(new PutUpdateJobStatusToInProgressRequest()
            {
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }

        private void TriggerCacheRefresh(HttpContext ctx)
        {
            int triggerSessionMinutes = (_requestSettings.Value.RequestsSessionExpiryInMinutes + 1) * -1;
            ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.AddMinutes(triggerSessionMinutes).ToString());
            ctx.Session.SetString("openJobsLastUpdated", DateTime.Now.AddMinutes(triggerSessionMinutes).ToString());
        }

        public async Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpSource source)
        {
            return await _requestHelpBuilder.GetSteps(source);
        }

    }
 }



