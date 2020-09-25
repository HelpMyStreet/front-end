using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Feedback
{
    public class FeedbackCaptureViewComponentParameters
    {
        public int JobId { get; set; }
        public RequestRoles RequestRole { get; set; }
    }
}
