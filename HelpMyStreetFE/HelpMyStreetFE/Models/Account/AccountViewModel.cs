using System.Collections.Generic;
using HelpMyStreetFE.Enums.Account;

namespace HelpMyStreetFE.Models.Account
{
    public class AccountViewModel
    {
        public MenuPage CurrentPage { get; set; }
        public dynamic PageModel { get; set; }
        public UserDetails UserDetails { get; set; }
        public List<NotificationModel> Notifications { get; set; }

    }
}
