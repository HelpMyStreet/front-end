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

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IOptions<RequestSettings> _requestSettings;
        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IOptions<RequestSettings> requestSettings)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestSettings = requestSettings;
        }
        public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel viewModel, int userId, HttpContext ctx)
        {
            _logger.LogInformation($"Logging Request");
            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest {                                     
                    AcceptedTerms = viewModel.HelpRequest.AcceptedTerms,
                    OtherDetails = viewModel.HelpRequest.OtherDetails,
                    ConsentForContact = viewModel.HelpRequest.ConsentForContact,
                    SpecialCommunicationNeeds = viewModel.HelpRequest.SpecialCommunicationNeeds,
                    ForRequestor = viewModel.HelpRequest.ForRequestor,
                    ReadPrivacyNotice = viewModel.HelpRequest.ReadPrivacyNotice,
                    CreatedByUserId = userId,
                    Recipient = new RequestPersonalDetails
                    {
                        FirstName = viewModel.HelpRequest.Recipient.Firstname,
                        LastName = viewModel.HelpRequest.Recipient.Lastname,
                        MobileNumber = viewModel.HelpRequest.Recipient.Mobile,
                        OtherNumber = viewModel.HelpRequest.Recipient.AltNumber,
                        EmailAddress = viewModel.HelpRequest.Recipient.Email,
                        Address = new Address
                        {
                            AddressLine1 = viewModel.HelpRequest.Recipient.Address.Addressline1,
                            AddressLine2 = viewModel.HelpRequest.Recipient.Address.Addressline2,
                            Locality = viewModel.HelpRequest.Recipient.Address.Locality,
                            Postcode = PostcodeFormatter.FormatPostcode(viewModel.HelpRequest.Recipient.Address.Postcode),
                        }
                    },
                    Requestor = new RequestPersonalDetails
                    {
                        FirstName = viewModel.HelpRequest.Requestor.Firstname,
                        LastName = viewModel.HelpRequest.Requestor.Lastname,
                        MobileNumber = viewModel.HelpRequest.Requestor.Mobile,
                        OtherNumber = viewModel.HelpRequest.Requestor.AltNumber,
                        EmailAddress = viewModel.HelpRequest.Requestor.Email,
                        Address = new Address
                        {
                            AddressLine1 = viewModel.HelpRequest.Requestor.Address.Addressline1,
                            AddressLine2 = viewModel.HelpRequest.Requestor.Address.Addressline2,
                            Locality = viewModel.HelpRequest.Requestor.Address.Locality,
                            Postcode = PostcodeFormatter.FormatPostcode(viewModel.HelpRequest.Requestor.Address.Postcode),
                        }
                    }
                },
                NewJobsRequest = new NewJobsRequest
                {
                    Jobs = new List<Job>
                    {
                        new Job
                        {
                            DueDays = viewModel.JobRequest.DueDays,
                            Details = viewModel.JobRequest.Details,
                            HealthCritical = viewModel.JobRequest.HealthCritical,
                            SupportActivity = (SupportActivities)Enum.Parse(typeof(SupportActivities), viewModel.JobRequest.SupportActivity),
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
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now) {
                jobs = (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId)).OrderOpenJobsForDisplay();           
                ctx.Session.SetObjectAsJson("acceptedJobs", jobs);
                ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.ToString());
            }

            return jobs;
    }


        public async Task<IDictionary<int, RequestContactInformation>> GetContactInformationForRequests(IEnumerable<int> ids)
        {
            List<GetJobDetailsResponse> details = new List<GetJobDetailsResponse>();

            foreach (var id in ids) {
                details.Add(await _requestHelpRepository.GetJobDetailsAsync(id));
            }

            return details.Aggregate(new Dictionary<int, RequestContactInformation>(), (acc, cur) =>
            {
                acc[cur.JobID] = new RequestContactInformation
                {
                    ForRequestor = cur.ForRequestor,
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
            var success =  await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
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
            var success =  await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
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

    }
}


