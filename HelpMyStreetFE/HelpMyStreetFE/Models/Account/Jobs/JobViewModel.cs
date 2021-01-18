using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobViewModel<T>
    {
        public T Item { get; set; }
        public bool UserHasRequiredCredentials { get; set; }
        public RequestRoles UserRole { get; set; }
        public bool HighlightJob { get; set; }
    }
}
