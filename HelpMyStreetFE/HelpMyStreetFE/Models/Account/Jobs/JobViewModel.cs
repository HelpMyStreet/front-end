using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobViewModel
    {
        public JobHeader JobHeader { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public bool UserActingAsAdmin { get; set; }
    }
}
