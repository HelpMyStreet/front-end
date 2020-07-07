using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreetFE.Repositories;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.GroupService.Request;
using System.Text;

namespace HelpMyStreetFE.Services
{
    public class GroupService : BaseHttpService, IGroupService
    { 

        public GroupService(
            IConfiguration configuration,
            HttpClient client) : base(client, configuration, "Services:Group")
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
    }
}