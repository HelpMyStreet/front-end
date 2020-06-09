using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.Validation;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.Stages.Detail
{
    [DetailStageValidation]
    public class RequestHelpDetailStageViewModel : IRequestHelpStageViewModel
    {
        public OrganisationDetails OrganisationRequestor { get; set; }
        public RequestorDetails Requestor { get; set; }
        public RecipientDetails Recipient { get; set; } 
        public string TemplateName { get; set; } = "RequestHelpDetailStageViewModel";
        public RequestorType Type { get; set; }
        public string CommunicationNeeds { get; set; }
        public string OtherDetails { get; set; }

        public bool ShowOtherDetails { get; set; } = true;
    }    
}

