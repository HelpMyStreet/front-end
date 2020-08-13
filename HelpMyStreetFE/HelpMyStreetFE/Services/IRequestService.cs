using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, HttpContext ctx); 
        Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, int maxOtherJobsToDisplay, User user, HttpContext ctx);
        Task<IEnumerable<JobSummary>> GetGroupRequestsAsync(int groupId, JobFilterRequest jobFilterRequest, HttpContext ctx);
        Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, HttpContext ctx);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId);
        Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, HttpContext ctx);
        Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, HttpContext ctx);
        Task<bool> UpdateJobStatusToCancelledAsync(int jobID, int createdByUserId, HttpContext ctx);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, HttpContext ctx);
        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source);
    }
}