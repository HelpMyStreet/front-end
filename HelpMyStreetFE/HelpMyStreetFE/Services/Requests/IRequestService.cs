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

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestService
    {
        Task<IEnumerable<JobHeader>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(string groupKey, bool waitForData, CancellationToken cancellationToken);
        Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken);
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken);
        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source);
        IEnumerable<JobHeader> SortAndFilterJobs(IEnumerable<JobHeader> jobs, JobFilterRequest jobFilterRequest);
        OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<JobHeader> jobs);
    }
}