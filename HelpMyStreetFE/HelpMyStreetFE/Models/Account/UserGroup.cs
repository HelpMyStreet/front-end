using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account
{
    public class UserGroup
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupKey { get; set; }
        public bool ShiftsEnabled { get; set; }
        public bool TasksEnabled { get; set; }
        public bool HomepageEnabled { get; set; }
        public IEnumerable<GroupRoles> UserRoles { get; set; }
    }
}
