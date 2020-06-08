using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Review
{
    public class RequestHelpReviewStageViewModel : IRequestHelpStageViewModel
    {
        public TasksViewModel Task { get; set; }
        public RequestorViewModel RequestedFor { get; set; }        
        public RecipientDetails Recipient  { get; set; }
        public RequestorDetails Requestor { get; set; }
        public RequestHelpTimeViewModel TimeRequested { get; set; }
        public bool? HealthCritical { get; set; }
        public string CommunicationNeeds { get; set; }
        public string OtherDetails { get; set; }

        public bool ShowOtherDetails { get; set; }

        public string HealthCriticalReviewString { get
            {
                if (HealthCritical.HasValue && HealthCritical.Value)
                    return "Health Critical";
                        return "Not Health Critical";
            }
        }

        public string TimeRequestedReviewString
        {
            get
            {
                if (TimeRequested == null) return "";
                if (TimeRequested.AllowCustom)
                {
                    if (TimeRequested.Days == 1) return $"Within {TimeRequested.Days} Day";
                    return $"Within {TimeRequested.Days} Days";
                }
                else
                {
                    return TimeRequested.TimeDescription;
                }
            }
        }
        public string TemplateName { get; set; } = "RequestHelpReviewStageViewModel";
    }
}
