using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;
using System;
using HelpMyStreet.Contracts;

namespace HelpMyStreetFE.Repositories
{
    public interface IRequestHelpRepository
    {
        Task<int> GetRequestId(int jobId);
        Task<Dictionary<int, int>> GetRequestIDs(IEnumerable<int> jobIDs);
        Task<PostRequestForHelpResponse> PostRequestForHelpAsync(PostRequestForHelpRequest request);
        Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> RequestIDs);
        Task<RequestSummary> GetRequestSummaryAsync(int requestId);
        Task<GetRequestDetailsResponse> GetRequestDetailsAsync(int requestId, int userId);
        Task<JobSummary> GetJobSummaryAsync(int jobId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId);
        Task<GetAllJobsByFilterResponse> GetAllJobsByFilterAsync(GetAllJobsByFilterRequest request);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToNewAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToDoneAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToOpenAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToCancelledAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToAcceptedAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<UpdateJobStatusOutcome?> PutUpdateShiftStatusToAccepted(int requestId, SupportActivities supportActivity, int createdByUserId, int volunteerUserId);
        Task<UpdateJobOutcome?> PutUpdateJobDueDate(int jobId, DateTime dueDate, int authorisedByUserID);
        Task<UpdateJobOutcome?> PutUpdateJobQuestion(int jobId, int questionId, string answer, int authorisedByUserID);
        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
        Task<IEnumerable<ShiftJob>> GetUserShiftJobsByFilter(GetUserShiftJobsByFilterRequest request);
        Task<IEnumerable<RequestSummary>> GetRequestsByFilter(GetRequestsByFilterRequest request);
        Task<IEnumerable<RequestSummary>> GetShiftRequestsByFilter(GetShiftRequestsByFilterRequest request);
        Task<UpdateJobStatusOutcome?> PutUpdateRequestStatusToDone(int requestId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> PutUpdateRequestStatusToCancelled(int requestId, int createdByUserId);
        Task<LogRequestEventResponse> LogEventRequest(LogRequestEventRequest request);
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
    }
}
