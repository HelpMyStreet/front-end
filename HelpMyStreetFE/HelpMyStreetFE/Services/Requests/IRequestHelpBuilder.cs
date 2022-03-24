using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestHelpBuilder
    {
        RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage, string alternativePostcode);
        RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage);
        RequestHelpViewModel GetSteps(RequestHelpJourney requestHelpJourney, int referringGroupId, string source, string language);   
        Task<List<RequestHelpQuestion>> GetQuestionsForTask(RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities supportActivity, int groupId);
    }
}
