using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        public async Task<LogRequestResponse> PostNewShifts(PostNewShiftsRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<LogRequestResponse>>("/api/PostNewShifts", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<GetJobSummaryResponse> GetJobSummaryAsync(int jobId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobSummaryResponse>>($"/api/GetJobSummary?jobID={jobId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
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

        public async Task<IEnumerable<JobHeader>> GetJobsByFilterAsync(GetJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetJobsByFilterResponse>>($"/api/GetJobsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.JobHeaders;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToNewAsync(int jobId, int createdByUserId)
        {
            var request = new PutUpdateJobStatusToNewRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToNewResponse>>($"/api/PutUpdateJobStatusToNew", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToDoneAsync(int jobId, int createdByUserId)
        {
            var request = new PutUpdateJobStatusToDoneRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToDoneResponse>>($"/api/PutUpdateJobStatusToDone", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToOpenAsync(int jobId, int createdByUserId)
        {
            var request = new PutUpdateJobStatusToOpenRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToOpenResponse>>($"/api/PutUpdateJobStatusToOpen", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToCancelledAsync(int jobId, int createdByUserId)
        {
            var request = new PutUpdateJobStatusToCancelledRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToCancelledResponse>>($"/api/PutUpdateJobStatusToCancelled", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToAcceptedAsync(int jobId, int createdByUserId, int volunteerUserId)
        {
            //var request = new PutUpdateJobStatusToAcceptedRequest()
            //{
            //    JobID = jobId,
            //    CreatedByUserID = createdByUserId,
            //    VolunteerUserID = volunteerUserId
            //};

            //var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToAcceptedResponse>>($"/api/PutUpdateJobStatusToAccepted", request);

            //if (response.HasContent && response.IsSuccessful)
            //{
            //    return response.Content.Outcome;
            //}
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId)
        {
            var request = new PutUpdateJobStatusToInProgressRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToInProgressResponse>>($"/api/PutUpdateJobStatusToInProgress", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> PutUpdateShiftStatusToAccepted(int requestId, SupportActivities supportActivity, int createdByUserId, int volunteerUserId)
        {
            var request = new PutUpdateShiftStatusToAcceptedRequest()
            {
                RequestID = requestId,
                SupportActivity = new SingleSupportActivityRequest() { SupportActivity = supportActivity},
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateShiftStatusToAcceptedResponse>>($"/api/PutUpdateShiftStatusToAccepted", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
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

        public async Task<IEnumerable<ShiftJob>> GetUserShiftJobsByFilter(GetUserShiftJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetUserShiftJobsByFilterResponse>>($"/api/GetUserShiftJobsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ShiftJobs;
            }
            return null;
        }

        public async Task<IEnumerable<ShiftJob>> GetOpenShiftJobsByFilter(GetOpenShiftJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetOpenShiftJobsByFilterResponse>>($"/api/GetOpenShiftJobsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                var jobs = response.Content.ShiftJobs;

                return jobs.Distinct(new ShiftJob_EqualityComparer());
            }
            return null;
        }

        public async Task<IEnumerable<RequestSummary>> GetShiftRequestsByFilter(GetShiftRequestsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetShiftRequestsByFilterResponse>>($"/api/GetShiftRequestsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.RequestSummaries;
            }
            return null;
        }

        public async Task<GetRequestDetailsResponse> GetRequestDetailsAsync(int requestId, int userId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetRequestDetailsResponse>>($"/api/GetRequestDetails?requestId={requestId}&authorisedByUserId={userId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        private class ShiftJob_EqualityComparer : IEqualityComparer<ShiftJob>
        {
            public bool Equals(ShiftJob a, ShiftJob b)
            {
                return a.RequestID == b.RequestID && a.SupportActivity == b.SupportActivity;

            }

            public int GetHashCode([DisallowNull] ShiftJob obj)
            {
                return obj.RequestID.GetHashCode() + obj.SupportActivity.GetHashCode();
            }
        }
    }
}
