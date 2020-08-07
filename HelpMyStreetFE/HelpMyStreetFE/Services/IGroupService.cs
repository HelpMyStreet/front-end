using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGroupService
    {
        Task<int> GetGroupIdByKey(string groupKey);

        Task<RegistrationFormVariant?> GetRegistrationFormVariant(int groupId, string source = "");

        Task<RequestHelpFormVariant?> GetRequestHelpFormVariant(int groupId, string source = "");

        Task AddUserToDefaultGroups(int userId);

        Task<List<int>> GetUserGroups(int userId);

        Task<List<UserGroup>> GetUserGroupRoles(int userId);
    }
}