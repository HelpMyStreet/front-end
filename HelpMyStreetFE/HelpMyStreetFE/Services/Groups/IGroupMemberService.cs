using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;

namespace HelpMyStreetFE.Services.Groups
{
    public interface IGroupMemberService
    {
        Task AddUserToDefaultGroups(int userId);

        Task<List<int>> GetUserGroups(int userId);
        Task<List<UserGroup>> GetUserGroupRoles(int userId, CancellationToken cancellationToken);
        Task<UserInGroup> GetGroupMember(int groupId, int userId, int authorisingUserId, CancellationToken cancellationToken);
        Task<List<UserInGroup>> GetAllGroupMembers(int groupId, int authorisingUserId);

        Task<bool> GetUserHasRole(int userId, int groupId, GroupRoles role, CancellationToken cancellationToken);
        Task<bool> GetUserHasRole(int userId, string groupKey, GroupRoles role, CancellationToken cancellationToken);
        bool GetUserHasRole(List<UserGroup> userGroupRoles, string groupKey, GroupRoles role);
        Task<bool> GetUserHasRole_Any(int userId, int groupId, IEnumerable<GroupRoles> rolesRequested, CancellationToken cancellationToken);
        Task<bool> GetUserHasRole_Any(int userId, string groupKey, IEnumerable<GroupRoles> rolesRequested, CancellationToken cancellationToken);
        bool GetUserHasRole_Any(List<UserGroup> userGroupRoles, string groupKey, IEnumerable<GroupRoles> rolesRequested);

        Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken);
        Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken);

        Task<AnnotatedGroupActivityCredentialSets> GetAnnotatedGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, int userId, int authorisingUserId, CancellationToken cancellationToken);
        Task<bool> GetUserHasCredentials(int groupId, SupportActivities supportActivitiy, int userId, int authorisingUserId, CancellationToken cancellationToken);
        Task<bool> PutGroupMemberCredentials(PutGroupMemberCredentialsRequest putGroupMemberCredentialsRequest);
        Task<bool> GetUserIsVerified(int userId, CancellationToken cancellationToken);
    }
}
