using System;
using System.Collections.Generic;
namespace HelpMyStreetFE.Models.Home
{
    public enum MessageType
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
        public MessageType Type { get; set; }
    }
}
