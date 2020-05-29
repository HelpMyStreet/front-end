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

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
        }
        public Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel viewModel, int userId)
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
            return _requestHelpRepository.PostNewRequestForHelpAsync(request);
        }
        public async Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, User user)
        {
           var all = await _requestHelpRepository.GetJobsByFilterAsync(user.PostalCode, distanceInMiles);

            var criteriaJobs = all.Where(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles < user.SupportRadiusMiles).OrderOpenJobsForDisplay();
            var otherJobs = all.Where(x => !criteriaJobs.Contains(x)).OrderOpenJobsForDisplay();

            var viewModel = new OpenJobsViewModel
            {
                CriteriaJobs = criteriaJobs,
                OtherJobs = otherJobs
            };
            return viewModel;          
        }
  
        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId)
        {
            return (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId))
                .OrderBy(j => j.DueDate)
                .ThenByDescending(j => j.IsHealthCritical)
                .ToList();
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
        public async Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId)
        {
            return await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
            {
                JobID = jobID,
                CreatedByUserID = createdByUserId
            });
        }
        public async Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId)
        {
            return await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });
        }
        public async Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId)
        {
            return await _requestHelpRepository.UpdateJobStatusToInProgressAsync(new PutUpdateJobStatusToInProgressRequest()
            {
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId,
                JobID = jobID
            });
        }
        
    }
}


