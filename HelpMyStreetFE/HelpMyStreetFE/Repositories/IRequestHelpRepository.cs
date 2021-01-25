using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Repositories
{
    public interface IRequestHelpRepository
    {
        Task<LogRequestResponse> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request);
        Task<GetRequestDetailsResponse> GetRequestDetailsAsync(int requestId, int userId);
        Task<GetJobSummaryResponse> GetJobSummaryAsync(int jobId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId);
        Task<IEnumerable<JobHeader>> GetJobsByFilterAsync(GetJobsByFilterRequest request);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToNewAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToDoneAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToOpenAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToCancelledAsync(int jobId, int createdByUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToAcceptedAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<UpdateJobStatusOutcome?> PutUpdateShiftStatusToAccepted(int requestId, SupportActivities supportActivity, int createdByUserId, int volunteerUserId);
        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
        Task<IEnumerable<ShiftJob>> GetUserShiftJobsByFilter(GetUserShiftJobsByFilterRequest request);
        Task<IEnumerable<ShiftJob>> GetOpenShiftJobsByFilter(GetOpenShiftJobsByFilterRequest request);
        Task<IEnumerable<RequestSummary>> GetShiftRequestsByFilter(GetShiftRequestsByFilterRequest request);
        Task<LogRequestResponse> PostNewShifts(PostNewShiftsRequest request);
        Task<UpdateJobStatusOutcome?> PutUpdateRequestStatusToCancelled(int requestId, int createdByUserId);

        
    }
}


