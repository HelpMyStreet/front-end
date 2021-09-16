using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestUpdatingService
    {
        Task<Fulfillable> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, string language, User user, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateRequestStatusAsync(int requestId, JobStatuses status, int createdByUserId, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken);
        Task<UpdateJobOutcome?> UpdateJobQuestion(int jobId, int questionId, string answer, int authorisedByUserId, CancellationToken cancellationToken);
    }
}