using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;

namespace HelpMyStreetFE.Models.Account
{
    public class AnnotatedGroupCredential
    {
        public AnnotatedGroupCredential(GroupCredential groupCredential, bool userHasCredential)
        {
            GroupCredential = groupCredential;
            UserHasCredential = userHasCredential;
        }

        public AnnotatedGroupCredential(GroupCredential groupCredential, List<int> userCredentials)
        {
            GroupCredential = groupCredential;
            UserHasCredential = userCredentials.Contains(groupCredential.CredentialID);
        }

        public GroupCredential GroupCredential { get; set; }
        public bool UserHasCredential { get; set; }
    }
}
