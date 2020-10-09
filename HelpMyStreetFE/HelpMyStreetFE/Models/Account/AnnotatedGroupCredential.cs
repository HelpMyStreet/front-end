using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;

namespace HelpMyStreetFE.Models.Account
{
    public class AnnotatedGroupCredential
    {
        public GroupCredential GroupCredential { get; set; }
        public bool UserHasCredential { get; set; }
    }
}
