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
        Task<IEnumerable<JobSummary>> GetJobsForUser(int userId);
        Task<IEnumerable<JobSummary>> GetOpenJobs(string pc, double distance);
        Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel request, int userId);

    }
}