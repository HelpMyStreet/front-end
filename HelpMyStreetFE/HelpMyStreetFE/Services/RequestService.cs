﻿using HelpMyStreet.Contracts.RequestService.Response;
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
            var request = new HelpMyStreet.Contracts.RequestService.Request.PostNewRequestForHelpRequest
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
                NewJobsRequest = new HelpMyStreet.Contracts.RequestService.Request.NewJobsRequest
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
            return _requestHelpRepository.LogRequest(request);
        }

        public async Task<IEnumerable<JobSummary>> GetOpenJobs(string pc, double distance)
        {
            var jobs = (await _requestHelpRepository.GetJobSummariesAsync(pc, distance))
                .OrderBy(j => j.DistanceInMiles)
                .ThenBy(j => j.DueDate)
                .ThenByDescending(j => j.IsHealthCritical)
                .ToList();

            return jobs;
        }
    }
}
