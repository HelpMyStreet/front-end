using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;

namespace HelpMyStreetFE.Services
{
    public class NewsTickersService : INewsTickersService
    {
        private readonly IGroupService _groupService;
        private readonly IRequestService _requestService;

        public NewsTickersService(IGroupService groupService, IRequestService requestService)
        {
            _groupService = groupService;
            _requestService = requestService;
        }
        public async Task<List<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            List<NewsTickerMessage> groupServiceMessages = await _groupService.GetNewsTickerMessages(groupId);

            List<NewsTickerMessage> requestServiceMessages = await _requestService.GetNewsTickerMessages(groupId);

            var messages = groupServiceMessages.Concat(requestServiceMessages).ToList();

            return messages;
        }
    }
}
