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
        public async Task<IEnumerable<JobSummary>> GetOpenJobsAsync(string postCode, double distanceInMiles)
        {
            var jobs = (await _requestHelpRepository.GetJobsByFilterAsync(postCode, distanceInMiles))
                .OrderBy(j => j.DistanceInMiles)
                .ThenBy(j => j.DueDate)
                .ThenByDescending(j => j.IsHealthCritical)
                .ToList();

            return jobs;
        }
        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId)
        {
            var jobs = (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId))
                .OrderBy(j => j.DueDate)
                .ThenByDescending(j => j.IsHealthCritical)
                .ToList();

            return jobs;
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
