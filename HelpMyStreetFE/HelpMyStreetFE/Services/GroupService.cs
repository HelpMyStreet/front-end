using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Account;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<List<UserGroup>> _memDistCache;
        private readonly IMemDistCache<int> _memDistCache_int;
        private readonly IMemDistCache<Group> _memDistCache_group;

        private const string CACHE_KEY_PREFIX = "group-service-";

        public GroupService(IGroupRepository groupRepository, IMemDistCache<List<UserGroup>> memDistCache, IMemDistCache<int> memDistCache_int, IMemDistCache<Group> memDistCache_group)
        {
            _groupRepository = groupRepository;
            _memDistCache = memDistCache;
            _memDistCache_int = memDistCache_int;
            _memDistCache_group = memDistCache_group;
        }

        public async Task<int> GetGroupIdByKey(string groupKey, CancellationToken cancellationToken)
        {
            return await _memDistCache_int.GetCachedDataAsync(async (cancellationToken) =>
            {
                var groupServiceResponse = await _groupRepository.GetGroupByKey(groupKey);

                if (groupServiceResponse == null) { throw new Exception($"Could not identify group for key '{groupKey}'"); }

                return groupServiceResponse.GroupId;
            }, $"{CACHE_KEY_PREFIX}-group-id-{groupKey}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<RegistrationFormVariant?> GetRegistrationFormVariant(int groupId, string source)
        {
            var groupServiceResponse = await _groupRepository.GetRegistrationFormVariant(groupId, source);

            return groupServiceResponse?.RegistrationFormVariant;
        }

        public async Task<RequestHelpFormVariant?> GetRequestHelpFormVariant(int groupId, string source)
        {
            var groupServiceResponse = await _groupRepository.GetRequestHelpFormVariant(groupId, source);

            return groupServiceResponse?.RequestHelpFormVariant;
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
                List<UserGroup> response = new List<UserGroup>();
                var userRoles = await _groupRepository.GetUserRoles(userId);

                foreach (var groupRoles in userRoles.UserGroupRoles)
                {
                    var group = await GetGroupById(groupRoles.Key, cancellationToken);
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
            }, $"{CACHE_KEY_PREFIX}-user-roles-user-{userId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<List<UserGroup>> GetGroupMembers(int groupId, int userId, CancellationToken cancellationToken)
        {
            var thisGroup = await GetGroupById(groupId, cancellationToken);
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

        public async Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken)
        {
            return await _memDistCache_group.GetCachedDataAsync(async (cancellationToken) =>
            {
                return (await _groupRepository.GetGroup(groupId)).Group;
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

        }
    }
}