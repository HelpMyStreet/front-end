using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Users;

namespace HelpMyStreetFE.Services.Groups
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<List<UserGroup>> _memDistCache;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        private const string CACHE_KEY_PREFIX = "group-member-service-";

        public GroupMemberService(IGroupRepository groupRepository, IMemDistCache<List<UserGroup>> memDistCache, IGroupService groupService, IUserService userService)
        {
            _groupRepository = groupRepository;
            _memDistCache = memDistCache;
            _groupService = groupService;
            _userService = userService;
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
                return await GetUserRoles(userId);
            }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<List<UserGroup>> GetGroupMembers(int groupId, int userId, CancellationToken cancellationToken)
        {
            var thisGroup = await _groupService.GetGroupById(groupId, cancellationToken);
            var groupMemberRoles = await _groupRepository.GetGroupMemberRoles(groupId, userId);

            List<UserGroup> response = new List<UserGroup>();
            foreach (var userRoles in groupMemberRoles.GroupMemberRoles)
            {
                response.Add(new UserGroup()
                {
                    UserId = userRoles.Key,
                    GroupId = groupId,
                    GroupKey = thisGroup.GroupKey,
                    GroupName = thisGroup.GroupName,
                    UserRoles = userRoles.Value.Select(role => (GroupRoles)role)
                });
            }

            return response;
        }

        public async Task<bool> GetUserHasRole(int userId, int groupId, GroupRoles role, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);

            return userGroupRoles?.Where(g => g.GroupId == groupId).FirstOrDefault()?.UserRoles.Contains(role) ?? false;
        }

        public async Task<bool> GetUserHasRole(int userId, string groupKey, GroupRoles role, CancellationToken cancellationToken)
        {
            var userGroupRoles = await GetUserGroupRoles(userId, cancellationToken);

            return userGroupRoles?.Where(g => g.GroupKey == groupKey).FirstOrDefault()?.UserRoles.Contains(role) ?? false;
        }

        public bool GetUserHasRole(List<UserGroup> userGroupRoles, string groupKey, GroupRoles role)
        {
            return userGroupRoles?.Where(g => g.GroupKey == groupKey).FirstOrDefault()?.UserRoles.Contains(role) ?? false;
        }

        public async Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.PostAssignRole(userId, groupId, role, authorisedByUserID);

            if (result == GroupPermissionOutcome.Success)
            {
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRoles(userId);
                }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", cancellationToken);
            }

            return result;
        }

        public async Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID, CancellationToken cancellationToken)
        {
            var result = await _groupRepository.PostRevokeRole(userId, groupId, role, authorisedByUserID);

            if (result == GroupPermissionOutcome.Success)
            {
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRoles(userId);
                }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", cancellationToken);
            }

            return result;
        }

        private async Task<List<UserGroup>> GetUserRoles(int userId)
        {
            List<UserGroup> response = new List<UserGroup>();
            var userRoles = await _groupRepository.GetUserRoles(userId);

            foreach (var groupRoles in userRoles.UserGroupRoles)
            {
                var group = await _groupRepository.GetGroup(groupRoles.Key);
                var roles = groupRoles.Value.Select(role => (GroupRoles)role);

                response.Add(new UserGroup()
                {
                    UserId = userId,
                    GroupId = group.Group.GroupId,
                    GroupKey = group.Group.GroupKey,
                    GroupName = group.Group.GroupName,
                    UserRoles = roles
                });
            }

            return response;
        }

        public async Task<AnnotatedGroupActivityCredentialSets> GetAnnotatedGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, int userId, int authorisingUserId, CancellationToken cancellationToken)
        {
            var gacs = await _groupService.GetGroupActivityCredentials(groupId, supportActivitiy, cancellationToken);
            var groupMember = await _groupRepository.GetGroupMember(groupId, userId, authorisingUserId);

            if (gacs == null) { throw new Exception("Null response from GetGroupActivityCredentials"); }
            if (groupMember == null) { throw new Exception("Null response from GetGroupMember"); }

            // This will be done in the Group Service?
            var user = await _userService.GetUserAsync(userId, cancellationToken);
            if (user.IsVerified == true) { groupMember.ValidCredentials.Add(-1); }

            return new AnnotatedGroupActivityCredentialSets(gacs, groupMember.ValidCredentials);
        }
    }
}
