using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Contracts;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;

namespace HelpMyStreetFE.Services
{
    public class NewsTickersService : INewsTickersService
    {
        private readonly IMemDistCache<IEnumerable<NewsTickerMessage>> _memDistCache;
        private readonly IGroupService _groupService;
        private readonly IRequestService _requestService;
        private readonly IFeedbackService _feedbackService;
        private const string CACHE_KEY_PREFIX = "newsticker-service";

        public NewsTickersService(IMemDistCache<IEnumerable<NewsTickerMessage>> memDistCache, IGroupService groupService, IRequestService requestService, IFeedbackService feedbackService)
        {
            _memDistCache = memDistCache ?? throw new ArgumentNullException(nameof(memDistCache));
            _groupService = groupService;
            _requestService = requestService;
            _feedbackService = feedbackService;
        }
        public async Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
                {
                
                    IEnumerable<NewsTickerMessage> groupServiceMessages = await _groupService.GetNewsTickerMessages(groupId);

                    IEnumerable<NewsTickerMessage> requestServiceMessages = await _requestService.GetNewsTickerMessages(groupId);

                    IEnumerable<NewsTickerMessage> feedbackServiceMessages = await _feedbackService.GetNewsTickerMessages(groupId);

                    return groupServiceMessages
                        .Concat(requestServiceMessages)
                        .Concat(feedbackServiceMessages);
                }, $"{CACHE_KEY_PREFIX}-group-{groupId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken, NotInCacheBehaviour.WaitForData);

                return result;
            }
            catch
            {
                // Don't break page if news ticker messages are missing
                return new List<NewsTickerMessage>();
            }
        }
    }
}
