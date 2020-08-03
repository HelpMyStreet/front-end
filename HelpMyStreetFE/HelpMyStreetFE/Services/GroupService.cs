using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreetFE.Repositories;
using HelpMyStreet.Contracts.GroupService.Request;

namespace HelpMyStreetFE.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<PostAssignRoleResponse> AssignRole(PostAssignRoleRequest postAssignRoleRequest)
        {
            return await _groupRepository.AssignRole(postAssignRoleRequest);
        }

        public async Task<GetChildGroupsResponse> GetChildGroups(int groupId)
        {
            return await _groupRepository.GetChildGroups(groupId);
        }

        public async Task<GetGroupResponse> GetGroup(int groupId)
        {
            return await _groupRepository.GetGroup(groupId);
        }

        public async Task<GetGroupByKeyResponse> GetGroupByKey(string groupKey)
        {
            return await _groupRepository.GetGroupByKey(groupKey);
        }

        public async Task<GetRegistrationFormVariantResponse> GetRegistrationFormVariant(int groupId, string source)
        {
            return await _groupRepository.GetRegistrationFormVariant(groupId, source);
        }

        public async Task<GetRequestHelpFormVariantResponse> GetRequestHelpFormVariant(int groupId, string source)
        {
            return await _groupRepository.GetRequestHelpFormVariant(groupId, source);
        }

        public async Task<GetUserGroupsResponse> GetUserGroups(int userId)
        {
            return await _groupRepository.GetUserGroups(userId);
        }

        public async Task<GetUserRolesResponse> GetUserRoles(int userId)
        {
            return await _groupRepository.GetUserRoles(userId);
        }

        public async Task<PostAddUserToDefaultGroupsResponse> PostAddUserToDefaultGroups(int userId)
        {
            return await _groupRepository.PostAddUserToDefaultGroups(userId);
        }
    }
}