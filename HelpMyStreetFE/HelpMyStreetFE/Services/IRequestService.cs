﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.RequestHelp;
using HelpMyStreetFE.Models.Account;
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
        Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, RequestHelpSource source, int userId, HttpContext ctx);
        Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId);
        Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, HttpContext ctx);
        Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, HttpContext ctx);
        Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, HttpContext ctx);
        Task<IDictionary<int, RequestContactInformation>> GetContactInformationForRequests(IEnumerable<int> ids);

        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpSource source);
    }
}