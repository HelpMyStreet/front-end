using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Contracts.ReportService.Request;
using HelpMyStreet.Contracts.ReportService.Response;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
		{
        }

	

		public async Task<PostRequestForHelpResponse> PostRequestForHelpAsync(PostRequestForHelpRequest request)
		{
			var response = await PostAsync<BaseRequestHelpResponse<PostRequestForHelpResponse>>("/api/PostRequestForHelp", request);
         
            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<LogRequestEventResponse> LogEventRequest(LogRequestEventRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<LogRequestEventResponse>>("/api/LogRequestEvent", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId)
        {
            var response = await GetAsync<BaseRequestHelpResponse<GetJobSummaryResponse>>($"/api/GetJobSummary?jobID={jobId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.JobSummary;
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

        public async Task<GetAllJobsByFilterResponse> GetAllJobsByFilterAsync(GetAllJobsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetAllJobsByFilterResponse>>($"/api/GetAllJobsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            throw new Exception("GetJobsByFilter call failed");
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
            var request = new PutUpdateJobStatusToAcceptedRequest()
            {
                JobID = jobId,
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobStatusToAcceptedResponse>>($"/api/PutUpdateJobStatusToAccepted", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
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

        public async Task<IEnumerable<RequestSummary>> GetShiftRequestsByFilter(GetShiftRequestsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetShiftRequestsByFilterResponse>>($"/api/GetShiftRequestsByFilter", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.RequestSummaries;
            }
            return null;
        }

        public async Task<IEnumerable<RequestSummary>> GetRequestsByFilter(GetRequestsByFilterRequest request)
        {
            var response = await PostAsync<BaseRequestHelpResponse<GetRequestsByFilterResponse>>($"/api/GetRequestsByFilter", request);

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

        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId)
        {
            var response = await GetRequestSummariesAsync(new List<int> { requestId });

            if (response != null && response.Count() == 1)
            {
                return response.First();
            }
            return null;
        }

        public async Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync (IEnumerable<int> requestIDs)
        {
            var request = new GetAllRequestsRequest { RequestIDs = requestIDs.ToList() };
            var response = await PostAsync<BaseRequestHelpResponse<GetAllRequestsResponse>>($"/api/GetAllRequests", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.RequestSummaries;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> PutUpdateRequestStatusToDone(int requestId, int createdByUserId)
        {
            var request = new PutUpdateRequestStatusToDoneRequest()
            {
                RequestID = requestId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateRequestStatusToDoneResponse>>($"/api/PutUpdateRequestStatusToDone", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobStatusOutcome?> PutUpdateRequestStatusToCancelled(int requestId, int createdByUserId)
        {
            var request = new PutUpdateRequestStatusToCancelledRequest()
            {
                RequestID = requestId,
                CreatedByUserID = createdByUserId
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateRequestStatusToCancelledResponse>>($"/api/PutUpdateRequestStatusToCancelled", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<int> GetRequestId(int jobId)
        {
            var d = await GetRequestIDs(new List<int> { jobId });

            if (d == null || d.Count == 0)
            {
                throw new Exception($"Could not find RequestId for jobid {jobId}");
            }

            return d.First().Value;
        }

        public async Task<Dictionary<int, int>> GetRequestIDs(IEnumerable<int> jobIDs)
        {
            var request = new GetRequestIDsRequest { JobIDs = jobIDs.ToList() };
            var response = await PostAsync<BaseRequestHelpResponse<GetRequestIDsResponse>>($"/api/GetRequestIDs", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.JobIDsToRequestIDs;
            }
            return null;
        }

        public async Task<UpdateJobOutcome?> PutUpdateJobDueDate(int jobId, DateTime dueDate, int authorisedByUserID)
        {
            var request = new PutUpdateJobDueDateRequest()
            {
                JobID = jobId,
                DueDate = dueDate,
                AuthorisedByUserID = authorisedByUserID
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobDueDateResponse>>($"/api/PutUpdateJobDueDate", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<UpdateJobOutcome?> PutUpdateJobQuestion(int jobId, int questionId, string answer, int authorisedByUserID)
        {
            var request = new PutUpdateJobQuestionRequest()
            {
                JobID = jobId,
                QuestionID = questionId,
                Answer = answer,
                AuthorisedByUserID = authorisedByUserID
            };

            var response = await PutAsync<BaseRequestHelpResponse<PutUpdateJobQuestionResponse>>($"/api/PutUpdateJobQuestion", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            return null;
        }

        public async Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            string url = "/api/GetNewsTicker";

            if (groupId.HasValue)
            {
                url += $"?groupId={groupId.Value}";
            }


            HttpResponseMessage response = await Client.GetAsync(url);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<NewsTickerResponse, RequestServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.Messages;
            }
            throw new Exception($"Bad response from GetNewsTicker for groupId {groupId}");
        }

        public async Task<Chart> GetChart(Charts chart, int groupId)
        {
            var request = new GetChartRequest
            {
                Chart = new HelpMyStreet.Contracts.GroupService.Request.ChartRequest()
                {
                    Chart = chart
                },
                GroupId = groupId
            };

            var response = await PostAsync<BaseRequestHelpResponse<GetChartResponse>>($"/api/GetChart", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Chart;
            }
            return null;
        }
    }
}
