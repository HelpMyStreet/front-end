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

	

		public async Task<BaseRequestHelpResponse<LogRequestResponse>> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request)
		{
			var response = await PostAsync<BaseRequestHelpResponse<LogRequestResponse>>("/api/PostNewRequestForHelp", request);
			return response;
		}
        
        public async Task<IEnumerable<JobSummary>> GetJobsAllocatedToUserAsync(int volunteerUserId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobsAllocatedToUserResponse>>($"/api/GetJobsAllocatedToUser?volunteerUserID={volunteerUserId}");

            return response.Content.JobSummaries;
        }

        public async Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobDetailsResponse>>($"/api/GetJobDetails?jobID={jobId}");

            return response.Content;
        }

        public async Task<IEnumerable<JobSummary>> GetJobsByFilterAsync(GetJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetJobsByFilterResponse>>($"/api/GetJobsByFilter", request);

            return response.Content.JobSummaries;
        }

        public async Task<bool> UpdateJobStatusToDoneAsync(PutUpdateJobStatusToDoneRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToDoneResponse>>($"/api/PutUpdateJobStatusToDone", request);

            return response.Content.Success;
        }

        public async Task<bool> UpdateJobStatusToOpenAsync(PutUpdateJobStatusToOpenRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToOpenResponse>>($"/api/PutUpdateJobStatusToOpen", request);

            return response.Content.Success;
        }

        public async Task<bool> UpdateJobStatusToInProgressAsync(PutUpdateJobStatusToInProgressRequest request)
        {
            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToInProgressResponse>>($"/api/PutUpdateJobStatusToInProgress", request);

            return response.Content.Success;
        }

        public async Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetQuestionsByActivtiesResponse>>($"/api/GetQuestionsByActivity", request);
            return response.Content;
        }
    }
}
