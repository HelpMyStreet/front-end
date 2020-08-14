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

	

		public async Task<LogRequestResponse> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request)
		{
			var response = await PostAsync<BaseRequestHelpResponse<LogRequestResponse>>("/api/PostNewRequestForHelp", request);
         
            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<IEnumerable<JobSummary>> GetJobsAllocatedToUserAsync(int volunteerUserId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobsAllocatedToUserResponse>>($"/api/GetJobsAllocatedToUser?volunteerUserID={volunteerUserId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.JobSummaries;
            }
            return null;
        }

        public async Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobDetailsResponse>>($"/api/GetJobDetails?jobID={jobId}&userID={userId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<GetJobStatusHistoryResponse> GetJobStatusHistoryAsync(int jobId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobStatusHistoryResponse>>($"/api/GetJobStatusHistory?jobID={jobId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<IEnumerable<JobSummary>> GetJobsByFilterAsync(GetJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetJobsByFilterResponse>>($"/api/GetJobsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.JobSummaries;
            }
            return null;
        }

        public async Task<bool> UpdateJobStatusToDoneAsync(PutUpdateJobStatusToDoneRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToDoneResponse>>($"/api/PutUpdateJobStatusToDone", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome == UpdateJobStatusOutcome.Success;
            }
            return false;
        }

        public async Task<bool> UpdateJobStatusToOpenAsync(PutUpdateJobStatusToOpenRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToOpenResponse>>($"/api/PutUpdateJobStatusToOpen", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome == UpdateJobStatusOutcome.Success;
            }
            return false;
        }

        public async Task<bool> UpdateJobStatusToCancelledAsync(PutUpdateJobStatusToCancelledRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToCancelledResponse>>($"/api/PutUpdateJobStatusToCancelled", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome == UpdateJobStatusOutcome.Success;
            }
            return false;
        }

        public async Task<bool> UpdateJobStatusToInProgressAsync(PutUpdateJobStatusToInProgressRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToInProgressResponse>>($"/api/PutUpdateJobStatusToInProgress", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome == UpdateJobStatusOutcome.Success;
            }
            return false;
        }

        public async Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetQuestionsByActivtiesResponse>>($"/api/GetQuestionsByActivity", request);
            
            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }
    }
}
