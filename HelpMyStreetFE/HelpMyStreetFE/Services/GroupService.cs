using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreetFE.Repositories;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.RequestService.Response;

namespace HelpMyStreetFE.Services
{
    public class GroupService : BaseHttpService, IGroupService
    { 

        public GroupService(
            IConfiguration configuration,
            HttpClient client) : base(client, configuration, "Services:Group")
        {
        }

        public async Task<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>> GetGroupByKey(string groupKey)
        {
            HttpResponseMessage response = await Client.GetAsync($"/api/GetGroupByKey?groupKey={groupKey}");
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>>(str);
        }

    }
}