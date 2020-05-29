using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId);
        Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, User user);
        Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel request, int userId);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId);
        Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId);
        Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId);
        Task<IDictionary<int, RequestContactInformation>> GetContactInformationForRequests(IEnumerable<int> ids);
    }
}