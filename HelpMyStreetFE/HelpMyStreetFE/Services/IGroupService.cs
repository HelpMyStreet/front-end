using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGroupService
    {
        Task<int> GetGroupIdByKey(string groupKey, CancellationToken cancellationToken);

        Task<RegistrationFormVariant?> GetRegistrationFormVariant(int groupId, string source = "");

        Task<RequestHelpFormVariant?> GetRequestHelpFormVariant(int groupId, string source = "");

        Task AddUserToDefaultGroups(int userId);

        Task<List<int>> GetUserGroups(int userId);

        Task<List<UserGroup>> GetUserGroupRoles(int userId, CancellationToken cancellationToken);

        Task<List<UserGroup>> GetGroupMembers(int groupId, int userId);

        Task<bool> GetUserHasRole(int userId, int groupId, GroupRoles role, CancellationToken cancellationToken);
        Task<bool> GetUserHasRole(int userId, string groupKey, GroupRoles role, CancellationToken cancellationToken);
        bool GetUserHasRole(List<UserGroup> userGroupRoles, string groupKey, GroupRoles role);

        Task<bool> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken);
        Task<bool> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken);
    }
}