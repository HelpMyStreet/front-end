using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, CancellationToken cancellationToken);
        Task<OpenJobsViewModel> GetOpenJobsAsync(User user, CancellationToken cancellationToken);
        Task<IEnumerable<JobSummary>> GetGroupRequestsAsync(int groupId, CancellationToken cancellationToken);
        Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, CancellationToken cancellationToken);
        Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, CancellationToken cancellationToken);
        Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, CancellationToken cancellationToken);
        Task<bool> UpdateJobStatusToCancelledAsync(int jobID, int createdByUserId, CancellationToken cancellationToken);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken);
        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source);
        IEnumerable<JobSummary> FilterJobs(IEnumerable<JobSummary> jobs, JobFilterRequest jobFilterRequest);
    }
}