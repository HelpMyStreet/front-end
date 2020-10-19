using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Volunteers
{
    public class AssignCredentialsViewModel
    {
        public User TargetUser { get; set; }
        public GroupCredential Credential { get; set; }

        public string Reference { get; set; }
        public string Notes { get; set; }
    }
}
