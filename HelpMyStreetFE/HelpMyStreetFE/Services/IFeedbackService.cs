using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Feedback;

namespace HelpMyStreetFE.Services
{
    public interface IFeedbackService
    {
        Task<bool> GetFeedbackExists(int jobId, RequestRoles requestRole);
        Task<FeedbackCaptureMessageViewModel.Messages> PostRecordFeedback(User user, CapturedFeedback feedback);
    }
}
