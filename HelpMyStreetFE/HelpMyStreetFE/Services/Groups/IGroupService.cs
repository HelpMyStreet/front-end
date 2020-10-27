using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
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

        Task<List<List<GroupCredential>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, CancellationToken cancellationToken);
        Task<List<GroupCredential>> GetGroupCredentials(int groupId);
        Task<GroupCredential> GetGroupCredential(int groupId, int credentialId);
    }
}