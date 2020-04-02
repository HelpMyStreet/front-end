using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account
{
    public class AccountViewModel
    {
        public UserDetails UserDetails { get; set; }
        public List<NotificationModel> Notifications { get; set; }
    }
}
