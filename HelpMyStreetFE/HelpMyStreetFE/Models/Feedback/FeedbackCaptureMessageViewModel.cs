using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Feedback
{
    public class FeedbackCaptureMessageViewModel
    {
        public enum Messages
        {
            Success,
            FeedbackAlreadyRecorded,
            IncorrectJobStatus,
            ServerError,
        }

        public Messages Message { get; set; }
    }
}
