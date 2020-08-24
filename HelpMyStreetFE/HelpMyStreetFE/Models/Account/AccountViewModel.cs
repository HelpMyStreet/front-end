using System.Collections.Generic;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account
{
    public class AccountViewModel
    {
        public MenuPage CurrentPage { get; set; }
        public UserGroup CurrentGroup { get; set; }
        public dynamic PageModel { get; set; }
        public UserDetails UserDetails { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<NotificationModel> Notifications { get; set; }
        public VerificationViewModel VerificationViewModel { get; set; }
        public List<JobSummary> Jobs { get; set; }
    }
}
