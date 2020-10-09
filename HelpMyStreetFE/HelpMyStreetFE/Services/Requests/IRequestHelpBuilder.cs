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
        RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage);
        RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage);
        Task<RequestHelpViewModel> GetSteps(RequestHelpFormVariant source2, int referringGroupId, string source);   
        Task<List<RequestHelpQuestion>> GetQuestionsForTask(RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities supportActivity);
    }
}
