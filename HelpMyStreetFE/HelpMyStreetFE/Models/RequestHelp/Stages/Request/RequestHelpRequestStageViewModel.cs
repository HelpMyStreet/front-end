using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpRequestStageViewModel : IRequestHelpStageViewModel
    {
 
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }          

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool? IsHealthCritical { get; set; }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToPrivacy { get; set; }

    }
}

