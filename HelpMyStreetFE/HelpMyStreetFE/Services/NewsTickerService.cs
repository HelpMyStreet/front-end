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
            var result = new List<NewsTickerMessage>();
            result.Add(new NewsTickerMessage() { Message = "**3,144 shops** completed", SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Shopping, Value = 3144 });
            result.Add(new NewsTickerMessage() { Message = "**586 wellbeing packages** delivered", SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.WellbeingPackage, Value = 586 });
            result.Add(new NewsTickerMessage() { Message = "**805 vaccination support shifts**", SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.VaccineSupport, Value = 586 });
            result.Add(new NewsTickerMessage() { Message = "**1,500+ volunteers**", SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.VolunteerSupport, Value = 586 });

            return result;
        }
    }
}
