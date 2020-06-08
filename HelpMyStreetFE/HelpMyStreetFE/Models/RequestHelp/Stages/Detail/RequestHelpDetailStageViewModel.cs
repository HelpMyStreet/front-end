using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.Stages.Detail
{
    public class RequestHelpDetailStageViewModel : IRequestHelpStageViewModel
    {                
        public RequestorDetails Requestor { get; set; }
        public RecipientDetails Recipient { get; set; } // always will have a recipient, just will be the same as the requestor if forRequestor = true;
        public string TemplateName { get; set; } = "RequestHelpDetailStageViewModel";
        public RequestorType Type { get; set; }
        public string CommunicationNeeds { get; set; }
        public string OtherDetails { get; set; }
        public bool ShowOtherDetails { get; set; } = true;
    }    
}

