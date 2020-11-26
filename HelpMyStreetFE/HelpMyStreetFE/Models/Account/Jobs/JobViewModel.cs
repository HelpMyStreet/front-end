using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobViewModel
    {
        public JobHeader JobHeader { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public RequestRoles UserRole { get; set; }
    }
}
