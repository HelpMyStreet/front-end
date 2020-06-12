using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.RequestHelp;
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
        int? GetVolunteerUserID(RequestHelpRequestStageViewModel requestStage, RequestHelpSource source, int userId);
        Task<RequestHelpViewModel> GetSteps(RequestHelpSource source);   
        List<RequestorType> GetRequestorTypeQuestion(RequestHelpSource source, int questionId);
    }
}
