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
        public bool HealthCritical { get; set; }

        public string HealthCriticalReviewString { get
            {
                if (HealthCritical)
                    return "Yes";
                        return "No";
            }
        }

        public string TimeRequestedReviewString
        {
            get
            {
                if (TimeRequested == null) return "";
                if (TimeRequested.AllowCustom)
                {
                    if (TimeRequested.Days == 1) return $"{TimeRequested.Days} Day";
                    return $"{TimeRequested.Days} Days";
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
