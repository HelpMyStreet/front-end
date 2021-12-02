using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Contracts;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;

namespace HelpMyStreetFE.Services
{
    public class NewsTickersService : INewsTickersService
    {
        private readonly IMemDistCache<List<NewsTickerMessage>> _memDistCache;
        private readonly IGroupService _groupService;
        private readonly IRequestService _requestService;
        private readonly IFeedbackService _feedbackService;
        private const string CACHE_KEY_PREFIX = "newsticker-service";

        public NewsTickersService(IMemDistCache<List<NewsTickerMessage>> memDistCache,IGroupService groupService, IRequestService requestService, IFeedbackService feedbackService)
        {
            _memDistCache = memDistCache ?? throw new ArgumentNullException(nameof(memDistCache));
            _groupService = groupService;
            _requestService = requestService;
            _feedbackService = feedbackService;
        }
        public async Task<List<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            var result =  await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {                
                List<NewsTickerMessage> groupServiceMessages = await _groupService.GetNewsTickerMessages(groupId);

                List<NewsTickerMessage> requestServiceMessages = await _requestService.GetNewsTickerMessages(groupId);

                //List<NewsTickerMessage> feedbackServiceMessages = await _feedbackService.GetNewsTickerMessages(groupId);

                return groupServiceMessages
                    .Concat(requestServiceMessages)
                    //.Concat(feedbackServiceMessages)
                    .ToList();
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}",RefreshBehaviour.DontWaitForFreshData, CancellationToken.None, NotInCacheBehaviour.DontWaitForData);

            
            return result;            
        }
    }
}
