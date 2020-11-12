using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Feedback
{
    public class CapturedFeedback
    {
        public int JobId { get; set; }
        public RequestRoles RoleSubmittingFeedback { get; set; }

        [Required]
        public FeedbackRating FeedbackRating { get; set; }
        public string RecipientMessage { get; set; }
        public string RequestorMessage { get; set; }
        public string VolunteerMessage { get; set; }
        public string GroupMessage { get; set; }
        public string HMSMessage { get; set; }
    }
}
