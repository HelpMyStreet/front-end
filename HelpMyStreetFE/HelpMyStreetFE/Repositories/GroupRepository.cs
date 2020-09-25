using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.GroupService.Request;
using System.Text;
using Microsoft.Extensions.Logging;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Reponses;
using System;

namespace HelpMyStreetFE.Repositories
{
    public class GroupRepository : BaseHttpRepository, IGroupRepository
    {
        public GroupRepository(
            IConfiguration configuration,
            ILogger<BaseHttpRepository> logger,
            HttpClient client) : base(client, configuration, logger, "Services:Group")
        {
        }

        public async Task<PostAssignRoleResponse> AssignRole(PostAssignRoleRequest postAssignRoleRequest)
        {
            string json = JsonConvert.SerializeObject(postAssignRoleRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/PostAssignRole", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<PostAssignRoleResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetChildGroupsResponse> GetChildGroups(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetChildGroups?groupID={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetChildGroupsResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetGroupResponse> GetGroup(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroup?groupID={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetGroupByKeyResponse> GetGroupByKey(string groupKey)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupByKey?groupKey={groupKey}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetRegistrationFormVariantResponse> GetRegistrationFormVariant(int groupId, string source)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetRegistrationFormVariant?groupId={groupId}&source={source}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetRegistrationFormVariantResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetRequestHelpFormVariantResponse> GetRequestHelpFormVariant(int groupId, string source)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetRequestHelpFormVariant?groupId={groupId}&source={source}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetRequestHelpFormVariantResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetUserGroupsResponse> GetUserGroups(int userId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetUserGroups?userId={userId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetUserGroupsResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetUserRolesResponse> GetUserRoles(int userId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetUserRoles?userId={userId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetUserRolesResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetGroupMembersResponse> GetGroupMembers(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupMembers?groupId={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupMembersResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<PostAddUserToDefaultGroupsResponse> PostAddUserToDefaultGroups(int userId)
        {
            PostAddUserToDefaultGroupsRequest postAddUserToDefaultGroupsRequest = new PostAddUserToDefaultGroupsRequest() { UserID = userId };
            string json = JsonConvert.SerializeObject(postAddUserToDefaultGroupsRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/PostAddUserToDefaultGroups", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<PostAddUserToDefaultGroupsResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GetGroupMemberRolesResponse> GetGroupMemberRoles(int groupId, int userId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupMemberRoles?groupId={groupId}&userId={userId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupMemberRolesResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return null;
        }

        public async Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisedByUserID)
        {
            PostAssignRoleRequest postAssignRoleRequest = new PostAssignRoleRequest()
            {
                AuthorisedByUserID = authorisedByUserID,
                GroupID = groupId,
                UserID = userId,
                Role = new RoleRequest() { GroupRole = role }
            };

            var response = await PostAsync<BaseRequestHelpResponse<PostAssignRoleResponse>>($"/api/PostAssignRole", postAssignRoleRequest);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            throw new Exception("Bad response from PostAssignRole");
        }

        public async Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisedByUserID)
        {
            PostRevokeRoleRequest postRevokeRoleRequest = new PostRevokeRoleRequest()
            {
                AuthorisedByUserID = authorisedByUserID,
                GroupID = groupId,
                UserID = userId,
                Role = new RoleRequest() { GroupRole = role }
            };

            var response = await PostAsync<BaseRequestHelpResponse<PostAssignRoleResponse>>($"/api/PostRevokeRole", postRevokeRoleRequest);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Outcome;
            }
            throw new Exception("Bad response from PostRevokeRole");
        }
    }
}