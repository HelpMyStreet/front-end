using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    //TODO: Combine with JobViewModel?
    public class ShiftViewModel
    {
        public ShiftJob ShiftJob { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public RequestRoles UserRole { get; set; }
        public bool HighlightJob { get; set; }
    }
}
