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
    public class Testimonial
    {
        public string Tagline { get; set; }
        public string Message { get; set; }
        public string Person { get; set; }
        public FeedbackMessageType Type { get; set; }
        public string GroupKey { get; set; }
        public bool B2BFeedback { get; set; }

        private string language;
        public string Language
        {
            get {
                if (string.IsNullOrEmpty(language))
                {
                    return "English";
                }
                else
                {
                    return language;
                }
            }
            set { language = value; }
        }
    }
}
