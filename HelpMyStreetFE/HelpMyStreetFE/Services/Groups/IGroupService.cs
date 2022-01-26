using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;

namespace HelpMyStreetFE.Services.Groups
{
    public interface IGroupService
    {
        Task<Chart> GetChart(Charts chart, int groupId);
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
        Task<List<Group>> GetGroupsWithMapDetails(MapLocation mapLocation, CancellationToken cancellationToken);
        Task<int> GetGroupIdByKey(string groupKey, CancellationToken cancellationToken);
        Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken);
        Task<Group> GetGroupByKey(string groupKey, CancellationToken cancellationToken);
        Task<RegistrationFormVariant> GetRegistrationFormVariant(int groupId, string source, CancellationToken cancellationToken);
        Task<RequestHelpJourney> GetRequestHelpFormVariant(int groupId, string source = "");
        Task<List<List<GroupCredential>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, CancellationToken cancellationToken);
        Task<List<GroupCredential>> GetGroupCredentials(int groupId);
        Task<GroupCredential> GetGroupCredential(int groupId, int credentialId);
        Task<Instructions> GetGroupSupportActivityInstructions(int groupId, SupportActivities supportActivity, CancellationToken cancellationToken);
        Task<Dictionary<SupportActivities, Instructions>> GetAllGroupSupportActivityInstructions(int groupId, IEnumerable<SupportActivities> supportActivities, CancellationToken cancellationToken); 
        Task<List<Group>> GetChildGroups(int groupId);
        Task<List<SupportActivityDetail>> GetSupportActivitesForRegistrationForm(RegistrationFormVariant registrationFormVariant);
        Task<List<Location>> GetUserLocations(int userId);
    }
}