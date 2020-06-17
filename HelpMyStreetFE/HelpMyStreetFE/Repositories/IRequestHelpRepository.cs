using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
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
        Task<BaseRequestHelpResponse<LogRequestResponse>> PostNewRequestForHelpAsync(PostNewRequestForHelpRequest request);
        Task<IEnumerable<JobSummary>> GetJobsAllocatedToUserAsync(int volunteerUserId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId);
        Task<IEnumerable<JobSummary>> GetJobsByFilterAsync(GetJobsByFilterRequest request);
        Task<bool> UpdateJobStatusToDoneAsync(PutUpdateJobStatusToDoneRequest request);
        Task<bool> UpdateJobStatusToOpenAsync(PutUpdateJobStatusToOpenRequest request);
        Task<bool> UpdateJobStatusToInProgressAsync(PutUpdateJobStatusToInProgressRequest request);

        Task<GetQuestionsByActivtiesResponse> GetQuestionsByActivity(GetQuestionsByActivitiesRequest request);
        

        
    }
}


