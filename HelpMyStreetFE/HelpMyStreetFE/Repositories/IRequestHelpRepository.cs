using HelpMyStreetFE.Models.Reponses;
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
        Task<bool> UpdateJobStatusToDoneAsync(PutUpdateJobStatusToDoneRequest request);
        Task<bool> UpdateJobStatusToOpenAsync(PutUpdateJobStatusToOpenRequest request);
        Task<bool> UpdateJobStatusToCancelledAsync(PutUpdateJobStatusToCancelledRequest request);
        Task<bool> UpdateJobStatusToInProgressAsync(PutUpdateJobStatusToInProgressRequest request);
        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
    }
}


