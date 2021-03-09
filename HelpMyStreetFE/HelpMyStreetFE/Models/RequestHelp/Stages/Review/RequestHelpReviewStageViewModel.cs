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
        public string OrganisationName { get; set; }
        public RequestHelpTimeViewModel TimeRequested { get; set; }
        public List<RequestHelpQuestion> RequestStageQuestions { get; set; }
        public List<RequestHelpQuestion> DetailsStageQuestions { get; set; }
        public bool ShowRequestor { get; set; }

        public string TimeRequestedReviewString
        {
            get
            {
                if (TimeRequested == null) return "";
                return TimeRequested.TimeDescription;
            }
        }
        public string FriendlyName { get; set; } = "Review";
        public string TemplateName { get; set; } = "RequestHelpReviewStageViewModel";
    }
}
