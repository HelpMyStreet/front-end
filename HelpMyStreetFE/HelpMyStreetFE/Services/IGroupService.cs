using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGroupService
    {
        Task<GetGroupByKeyResponse> GetGroupByKey(string groupKey);

        Task<GetGroupResponse> GetGroup(int groupId);

        Task<GetChildGroupsResponse> GetChildGroups(int groupId);

        Task<PostAssignRoleResponse> AssignRole(PostAssignRoleRequest postAssignRoleRequest);

        Task<GetRegistrationFormVariantResponse> GetRegistrationFormVariant(int groupId, string source = "");

        Task<GetRequestHelpFormVariantResponse> GetRequestFormVariant(int groupId, string source = "");

        Task<PostAddUserToDefaultGroupsResponse> PostAddUserToDefaultGroups(int userId);
    }
}