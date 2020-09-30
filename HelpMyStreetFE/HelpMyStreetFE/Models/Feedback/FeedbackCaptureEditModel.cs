using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.Models.Feedback
{
    public class FeedbackCaptureEditModel
    {
        [Required]
        public RequestRoles RoleSubmittingFeedback { get; set; }

        public bool ShowRecipientMessage { get; set; }
        public bool ShowRequestorMessage { get; set; }
        public bool ShowVolunteerMessage { get; set; }
        public bool ShowGroupMessage { get; set; }
        public bool ShowHMSMessage { get; set; }

        public string RecipientName { get; set; }
        public string RequestorName { get; set; }
        public string VolunteerName { get; set; }
        public string GroupName { get; set; }

        [Required]
        [Range(1, 2)]
        public int FeedbackRating { get; set; }
        public string RecipientMessage { get; set; }
        public string RequestorMessage { get; set; }
        public string VolunteerMessage { get; set; }
        public string GroupMessage { get; set; }
        public string HMSMessage { get; set; }
    }
}
