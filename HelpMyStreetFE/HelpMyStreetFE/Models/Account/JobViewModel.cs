using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account
{
    public class JobViewModel
    {
        public JobSummary JobSummary { get; set; }
        public bool UserIsVerified { get; set; }
        public bool UserActingAsAdmin { get; set; }
        public RequestContactInformation ContactInformation { get; set; }
    }
}
