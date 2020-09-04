using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Account;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System;
using HelpMyStreet.Cache;
using System.Threading;

namespace HelpMyStreetFE.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<List<UserGroup>> _memDistCache;

        private const string CACHE_KEY_PREFIX = "group-service-user-roles";

        public GroupService(IGroupRepository groupRepository, IMemDistCache<List<UserGroup>> memDistCache)
        {
            _groupRepository = groupRepository;
            _memDistCache = memDistCache;
        }

        public async Task<int> GetGroupIdByKey(string groupKey)
        {
            var groupServiceResponse = await _groupRepository.GetGroupByKey(groupKey);

            if (groupServiceResponse == null) { throw new Exception($"Could not identify group for key '{groupKey}'"); }

            return groupServiceResponse.GroupId;
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
            }, $"{CACHE_KEY_PREFIX}-user-{userId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<List<UserGroup>> GetGroupMembers(int groupId, int userId)
        {
            var thisGroup = (await _groupRepository.GetGroup(groupId)).Group;
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
    }
}