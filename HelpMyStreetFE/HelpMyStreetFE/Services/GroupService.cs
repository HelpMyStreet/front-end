using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Account;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System;

namespace HelpMyStreetFE.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
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

        public async Task<List<UserGroup>> GetUserGroupRoles(int userId)
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

        public async Task<List<UserGroup>> GetGroupMembers(int groupId)
        {
            var thisGroup = (await _groupRepository.GetGroup(groupId)).Group;
            List<int> userIds = (await _groupRepository.GetGroupMembers(groupId)).Users;

            List<UserGroup> response = new List<UserGroup>();
            foreach (int userId in userIds)
            {
                // TODO: Remove this call (once we have GetGroupMemberRoles)
                var userRoles = await _groupRepository.GetUserRoles(userId);
                response.AddRange(userRoles.UserGroupRoles.Where(group => group.Key == groupId).Select(group => new UserGroup()
                {
                    UserId = userId,
                    GroupId = groupId,
                    GroupKey = thisGroup.GroupKey,
                    GroupName = thisGroup.GroupName,
                    UserRoles = group.Value.Select(role => (GroupRoles)role)
                }));
            }

            return response;
        }

    }
}