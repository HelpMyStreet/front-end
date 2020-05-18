using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
	public class RequestHelpRepository : BaseHttpRepository, IRequestHelpRepository
	{
		public RequestHelpRepository(
			HttpClient client,
			IConfiguration config,
			ILogger<RequestHelpRepository> logger) : base(client, config, logger, "Services:Request")
		{ }

	

		public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequest(PostNewRequestForHelpRequest request)
		{
			var response = await PostAsync<BaseRequestHelpResponse<LogRequestResponse>>("/api/PostNewRequestForHelp", request);
			return response;
		}

		public async Task<IEnumerable<JobSummary>> GetJobSummariesAsync(string postCode, double distanceInMiles)
		{
			/*
			var response = await GetAsync<BaseRequestHelpResponse<GetJobsByFilterResponse>>($"/api/PostNewRequestForHelp?postcode=${postCode}&distanceInMiles=${distanceInMiles}");

			return response.Content.JobSummaries;
			*/

			return new List<JobSummary>
            {
                new JobSummary {
                    UniqueIdentifier = Guid.NewGuid(),
                    IsHealthCritical = false,
                    DueDate = new DateTime(2020, 05, 24), // should be last
                    SupportActivity = SupportActivities.CollectingPrescriptions,
                    PostCode = "AB1 2CD",
                    DistanceInMiles = 1.23
                },
                new JobSummary {
                    UniqueIdentifier = Guid.NewGuid(),
                    IsHealthCritical = false,
                    DueDate = new DateTime(2020, 05, 22), // should be 2nd
                    SupportActivity = SupportActivities.Shopping,
                    PostCode = "AB1 2CD",
                    DistanceInMiles = 1.23
                },
                new JobSummary {
                    UniqueIdentifier = Guid.NewGuid(),
                    IsHealthCritical = false,
                    DueDate = new DateTime(2020, 05, 25),
                    SupportActivity = SupportActivities.Other,
                    PostCode = "AB1 2CD",
                    DistanceInMiles = 1 // should be first
                },
                new JobSummary {
                    UniqueIdentifier = Guid.NewGuid(),
                    IsHealthCritical = true, // should be 3rd
                    DueDate = new DateTime(2020, 05, 24),
                    SupportActivity = SupportActivities.Other,
                    PostCode = "AB1 2CD",
                    DistanceInMiles = 1.23
                },
            };
		}
	}
}
