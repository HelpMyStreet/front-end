using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IRequestHelpBuilder
    {
        RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage);
        RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage);
        int? GetVolunteerUserID(RequestHelpRequestStageViewModel requestStage, RequestorType type, RequestHelpFormVariant requestHelpFormVariant, int userId);
        Task<RequestHelpViewModel> GetSteps(RequestHelpFormVariant source2, int referringGroupId, string source);   
        List<RequestorType> GetRequestorTypeQuestion(RequestHelpFormVariant source, int questionId);
    }
}
