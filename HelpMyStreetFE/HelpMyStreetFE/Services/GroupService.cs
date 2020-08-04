using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreetFE.Repositories;
using HelpMyStreet.Contracts.GroupService.Request;
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
                var roles = groupRoles.Value.Select(role => (GroupRoles)role).ToList();

                response.Add(new UserGroup()
                {
                    GroupId = group.Group.GroupId,
                    GroupKep = group.Group.GroupKey,
                    GroupName = group.Group.GroupName,
                    UserRoles = roles
                });
            }

            return response;
        }
    }
}