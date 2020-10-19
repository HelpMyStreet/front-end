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
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

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
            throw new Exception($"Bad response from GetGroup for GroupId {groupId}");
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

        public async Task<UserInGroup> GetGroupMember(int groupId, int userId, int authorisingUserId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupMember?groupId={groupId}&userId={userId}&authorisingUserId={authorisingUserId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupMemberResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.UserInGroup;
            }
            return null;
        }

        public async Task<List<UserInGroup>> GetAllGroupMembers(int groupId, int authorisingUserId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetAllGroupMembers?groupId={groupId}&authorisingUserId={authorisingUserId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetAllGroupMembersResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.UserInGroups;
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

        public async Task<List<List<int>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivitiy)
        {
            var getGroupActivityCredentialsRequest = new GetGroupActivityCredentialsRequest()
            {
                GroupId = groupId,
                SupportActivityType = new SupportActivityType() { SupportActivity = supportActivitiy },
            };

            string json = JsonConvert.SerializeObject(getGroupActivityCredentialsRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetGroupActivityCredentials", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupActivityCredentialsResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.CredentialSets;
            }
            return null;
        }

        public async Task<List<GroupCredential>> GetGroupCredentials(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupCredentials?groupId={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetGroupCredentialsResponse, GroupServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.GroupCredentials;
            }
            throw new Exception("Bad response from GetGroupCredentials");
        }
    }
}