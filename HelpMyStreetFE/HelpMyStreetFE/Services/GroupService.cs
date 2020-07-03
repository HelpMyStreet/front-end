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

        public async Task<ResponseWrapper<PostAssignRoleResponse, GroupServiceErrorCode>> AssignRole(PostAssignRoleRequest postAssignRoleRequest)
        {
            string json = JsonConvert.SerializeObject(postAssignRoleRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/PostAssignRole", data);
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<PostAssignRoleResponse, GroupServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetChildGroupsResponse, GroupServiceErrorCode>> GetChildGroups(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetChildGroups?groupID={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetChildGroupsResponse, GroupServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetGroupResponse, GroupServiceErrorCode>> GetGroup(int groupId)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroup?groupID={groupId}");
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetGroupResponse, GroupServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>> GetGroupByKey(string groupKey)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupByKey?groupKey={groupKey}");
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>>(str);
        }

    }
}