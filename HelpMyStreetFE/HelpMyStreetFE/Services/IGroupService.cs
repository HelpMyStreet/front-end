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

        Task<List<UserGroup>> GetGroupMembers(int groupId);

        Task<bool> GetUserHasRole(int userId, int groupId, GroupRoles role);
        Task<bool> GetUserHasRole(int userId, string groupKey, GroupRoles role);
        bool GetUserHasRole(List<UserGroup> userGroupRoles, string groupKey, GroupRoles role);
    }
}