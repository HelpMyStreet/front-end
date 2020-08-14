using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Volunteers
{
    public class VolunteerViewModel
    {
        public User User { get; set; }
        public IEnumerable<GroupRoles> Roles { get; set; }
    }
}
