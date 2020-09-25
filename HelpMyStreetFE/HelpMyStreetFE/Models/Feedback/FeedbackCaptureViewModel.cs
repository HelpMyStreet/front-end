using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Feedback
{
    public class FeedbackCaptureViewModel
    {
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

    }
}
