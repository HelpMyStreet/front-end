using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Services.Groups
{
    public interface IGroupService
    {
        Task<int> GetGroupIdByKey(string groupKey, CancellationToken cancellationToken);

        Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken);

        Task<RegistrationFormVariant?> GetRegistrationFormVariant(int groupId, string source = "");

        Task<RequestHelpJourney> GetRequestHelpFormVariant(int groupId, string source = "");
    }
}