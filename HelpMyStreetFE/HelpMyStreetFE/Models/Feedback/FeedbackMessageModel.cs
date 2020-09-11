using System;
using System.Collections.Generic;
namespace HelpMyStreetFE.Models.Feedback
{
    public enum FeedbackMessageType
    {
        FaceCovering,
        Group,
        Other
    }
    public class FeedbackMessage
    {
        public string Tagline { get; set; }
        public string Message { get; set; }
        public string Person { get; set; }
        public FeedbackMessageType Type { get; set; }
        public string GroupKey { get; set; }
    }
}
