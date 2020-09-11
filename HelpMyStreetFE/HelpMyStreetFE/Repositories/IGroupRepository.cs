using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Repositories
{
    public interface IGroupRepository
    {
        Task<GetGroupByKeyResponse> GetGroupByKey(string groupKey);

        Task<GetGroupResponse> GetGroup(int groupId);

        Task<GetChildGroupsResponse> GetChildGroups(int groupId);

        Task<PostAssignRoleResponse> AssignRole(PostAssignRoleRequest postAssignRoleRequest);

        Task<GetRegistrationFormVariantResponse> GetRegistrationFormVariant(int groupId, string source = "");

        Task<GetRequestHelpFormVariantResponse> GetRequestHelpFormVariant(int groupId, string source = "");

        Task<PostAddUserToDefaultGroupsResponse> PostAddUserToDefaultGroups(int userId);

        Task<GetUserGroupsResponse> GetUserGroups(int userId);

        Task<GetUserRolesResponse> GetUserRoles(int userId);

        Task<GetGroupMembersResponse> GetGroupMembers(int groupId);

        Task<GetGroupMemberRolesResponse> GetGroupMemberRoles(int groupId, int authorisedByUserID);

        Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID);

        Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID);
    }
}
