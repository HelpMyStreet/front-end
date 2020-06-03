using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Interface;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models
{
    public class RequestHelpDetailStageViewModel : IRequestHelpStepsViewModel
    {                
        public RequestorDetails Requestor { get; set; }
        public RecipientDetails Recipient { get; set; } // always will have a recipient, just will be the same as the requestor if forRequestor = true;
        public string TemplateName { get; set; } = "RequestHelpDetailStageViewModel";
        public RequestorType Type { get; set; }
    }

    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MobileNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Email { get; set; }

        public string Postcode { get; set; }
    }

    public class RequestorDetails : Person
    {

    }
    public class RecipientDetails : Person
    {        
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
    }

    
}

