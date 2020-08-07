using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account
{
    public class UserGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupKey { get; set; }
        public List<GroupRoles> UserRoles { get; set; }
    }
}
