using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums;
using HelpMyStreetFE.Models.Feedback;

namespace HelpMyStreetFE.Services
{
    public interface IFeedbackService
    {
        Task<bool> GetFeedbackExists(int jobId, RequestRoles requestRole, int? userId);
        Task<Result> PostRecordFeedback(User user, CapturedFeedback feedback);

        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
    }
}
