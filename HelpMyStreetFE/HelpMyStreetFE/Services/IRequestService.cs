using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<JobSummary>> GetOpenJobs(string pc, double distance);
        Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel request, int userId);
        Task<IEnumerable<JobSummary>> GetJobsAllocatedToUserAsync(int volunteerUserId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId);
        Task<IEnumerable<JobSummary>> GetJobsByFilterAsync(string postCode, double distanceInMiles);
        Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId);
        Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId);

    }
}