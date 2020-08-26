using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
        Task<LogRequestResponse> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId);
        Task<GetJobStatusHistoryResponse> GetJobStatusHistoryAsync(int jobId);
        Task<IEnumerable<JobSummary>> GetJobsByFilterAsync(GetJobsByFilterRequest request);
        Task<bool> UpdateJobStatusToDoneAsync(int jobId, int createdByUserId);
        Task<bool> UpdateJobStatusToOpenAsync(int jobId, int createdByUserId);
        Task<bool> UpdateJobStatusToCancelledAsync(int jobId, int createdByUserId);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobId, int createdByUserId, int volunteerUserId);
        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
    }
}


