using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Volunteers
{
    public class VolunteerViewModel
    {
        public User User { get; set; }
        public int CompletedRequests { get; set; }
        public List<AnnotatedGroupCredential> Credentials { get; set; }
        public IEnumerable<GroupRoles> Roles { get; set; }
        public string RolesSummary
        {
            get
            {
                List<GroupRoles> rolesToExclude = new List<GroupRoles>() { GroupRoles.Member, GroupRoles.Volunteer };

                List<string> roles = new List<string>();

                if (Roles.Contains(GroupRoles.TaskAdmin) && Roles.Contains(GroupRoles.UserAdmin))
                {
                    roles.Add("Admin");

                    rolesToExclude.Add(GroupRoles.TaskAdmin);
                    rolesToExclude.Add(GroupRoles.UserAdmin);
                }

                roles.AddRange(Roles.Where(r => !rolesToExclude.Contains(r)).Select(r => r.FriendlyName()));

                return string.Join(", ", roles);
            }
        }
    }
}
