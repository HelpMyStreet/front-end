using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Feedback
{
    public class CapturedFeedback
    {
        public int JobId { get; set; }
        public RequestRoles RoleSubmittingFeedback { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FeedbackRating FeedbackRating { get; set; }
        public string RecipientMessage { get; set; }
        public string RequestorMessage { get; set; }
        public string VolunteerMessage { get; set; }
        public string GroupMessage { get; set; }
        public string HMSMessage { get; set; }

        public bool IncludesMessage
        {
            get
            {
                return !(string.IsNullOrEmpty(RecipientMessage)
                      && string.IsNullOrEmpty(RequestorMessage)
                      && string.IsNullOrEmpty(VolunteerMessage)
                      && string.IsNullOrEmpty(GroupMessage)
                      && string.IsNullOrEmpty(HMSMessage));
            }
        }
    }
}
