using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetailViewModel
    {
        public JobSummary JobSummary { get; set; }
        public bool UserIsVerified { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
    }
}
