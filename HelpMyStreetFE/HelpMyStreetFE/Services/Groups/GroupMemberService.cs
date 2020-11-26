using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.Services.Groups
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<List<UserGroup>> _memDistCache;
        private readonly IMemDistCache<UserInGroup> _memDistCache_userInGroup;
        private readonly IGroupService _groupService;

        private const string CACHE_KEY_PREFIX = "group-member-service-";
        private const int YOTI_CREDENTIAL_ID = -1;

        public GroupMemberService(IGroupRepository groupRepository, IMemDistCache<List<UserGroup>> memDistCache, IGroupService groupService, IMemDistCache<UserInGroup> memDistCache_userInGroup)
        {
            _groupRepository = groupRepository;
            _memDistCache = memDistCache;
            _groupService = groupService;
            _memDistCache_userInGroup = memDistCache_userInGroup;
        }

        public async Task AddUserToDefaultGroups(int userId)
        {
            var groupServiceResponse = await _groupRepository.PostAddUserToDefaultGroups(userId);

            if (groupServiceResponse == null || !groupServiceResponse.Success) { throw new Exception($"Could not add user {userId} to default groups"); }
        }

        public async Task<List<int>> GetUserGroups(int userId)
        {
            return (await _groupRepository.GetUserGroups(userId)).Groups;
        }

        public async Task<List<UserGroup>> GetUserGroupRoles(int userId, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserRoles(userId, cancellationToken);
            }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", RefreshBehaviour.WaitForFreshData, cancellationToken);
        }

        public async Task<bool> GetUserHasRole(int userId, int groupId, GroupRoles role, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);

            return GetUserHasRole(userGroupRoles, group.GroupKey, role);
        }

        public async Task<bool> GetUserHasRole_Any(int userId, int groupId, IEnumerable<GroupRoles> rolesRequested, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);
            var group = await _groupService.GetGroupById(groupId, cancellationToken);

            return GetUserHasRole_Any(userGroupRoles, group.GroupKey, rolesRequested);
        }

        public async Task<bool> GetUserHasRole(int userId, string groupKey, GroupRoles role, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);

            return GetUserHasRole(userGroupRoles, groupKey, role);
        }
        public async Task<bool> GetUserHasRole_Any(int userId, string groupKey, IEnumerable<GroupRoles> rolesRequested, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);

            return GetUserHasRole_Any(userGroupRoles, groupKey, rolesRequested);
        }

        public bool GetUserHasRole(List<UserGroup> userGroupRoles, string groupKey, GroupRoles role)
        {
            return userGroupRoles?.Where(g => g.GroupKey == groupKey).FirstOrDefault()?.UserRoles.Contains(role) ?? false;
        }

        public bool GetUserHasRole_Any(List<UserGroup> userGroupRoles, string groupKey, IEnumerable<GroupRoles> rolesRequested)
        {
            return rolesRequested.Any(role => GetUserHasRole(userGroupRoles, groupKey, role));
        }

        public async Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.PostAssignRole(userId, groupId, role, authorisedByUserID);

            if (result == GroupPermissionOutcome.Success)
            {
                // Cannot do this due to circular dependency
                //_requestService.TriggerCacheRefresh(userId, cancellationToken);

                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRoles(userId, cancellationToken);
                }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", cancellationToken);
            }

            return result;
        }

        public async Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.PostRevokeRole(userId, groupId, role, authorisedByUserID);

            if (result == GroupPermissionOutcome.Success)
            {
                // Cannot do this due to circular dependency
                //_requestService.TriggerCacheRefresh(userId, cancellationToken);

                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRoles(userId, cancellationToken);
                }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", cancellationToken);
            }

            return result;
        }

        private async Task<List<UserGroup>> GetUserRoles(int userId, CancellationToken cancellationToken)
        {
            List<UserGroup> response = new List<UserGroup>();
            var userRoles = await _groupRepository.GetUserRoles(userId);

            foreach (var groupRoles in userRoles.UserGroupRoles)
            {
                var group = await _groupService.GetGroupById(groupRoles.Key, cancellationToken);
                var roles = groupRoles.Value.Select(role => (GroupRoles)role);

                response.Add(new UserGroup()
                {
                    UserId = userId,
                    GroupId = group.GroupId,
                    GroupKey = group.GroupKey,
                    GroupName = group.GroupName,
                    UserRoles = roles
                });
            }

            return response;
        }

        public async Task<UserInGroup> GetGroupMember(int groupId, int userId, int authorisingUserId, CancellationToken cancellationToken)
        {
            var result = await _memDistCache_userInGroup.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _groupRepository.GetGroupMember(groupId, userId, authorisingUserId);
            }, $"{CACHE_KEY_PREFIX}-group-member-{groupId}-{userId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

            return result;
        }

        public async Task<List<UserInGroup>> GetAllGroupMembers(int groupId, int authorisingUserId)
        {
            return await _groupRepository.GetAllGroupMembers(groupId, authorisingUserId);
        }

        public async Task<AnnotatedGroupActivityCredentialSets> GetAnnotatedGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, int userId, int authorisingUserId, CancellationToken cancellationToken)
        {
            var gacs = await _groupService.GetGroupActivityCredentials(groupId, supportActivitiy, cancellationToken);
            var groupMember = await GetGroupMember(groupId, userId, authorisingUserId, cancellationToken);

            if (gacs == null) { throw new Exception("Null response from GetGroupActivityCredentials"); }
            if (groupMember == null) { throw new Exception("Null response from GetGroupMember"); }

            return new AnnotatedGroupActivityCredentialSets(gacs, groupMember.ValidCredentials);
        }

        public async Task<bool> GetUserHasCredentials(int groupId, SupportActivities supportActivitiy, int userId, int authorisingUserId, CancellationToken cancellationToken)
        {
            var annotatedGacs = await GetAnnotatedGroupActivityCredentials(groupId, supportActivitiy, userId, authorisingUserId, cancellationToken);

            return annotatedGacs.AreSatisfied;
        }

        public async Task<bool> PutGroupMemberCredentials(PutGroupMemberCredentialsRequest putGroupMemberCredentialsRequest)
        {
            return await _groupRepository.PutGroupMemberCredentials(putGroupMemberCredentialsRequest);
        }

        public async Task<bool> GetUserIsVerified(int userId, CancellationToken cancellationToken)
        {
            var groupMember = await _groupRepository.GetGroupMember((int)HelpMyStreet.Utils.Enums.Groups.Generic, userId, userId);

            return groupMember.ValidCredentials.Contains(YOTI_CREDENTIAL_ID);
        }
    }
}
