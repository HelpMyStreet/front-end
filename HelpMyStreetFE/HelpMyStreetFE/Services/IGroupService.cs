using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGroupService
    {
        Task<ResponseWrapper<GetGroupByKeyResponse, GroupServiceErrorCode>> GetGroupByKey(string groupKey); 
    }
}