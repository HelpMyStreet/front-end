using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.GroupService.Response;

namespace HelpMyStreetFE.Models.Account
{
    public class AnnotatedGroupActivityCredentialSets
    {
        public AnnotatedGroupActivityCredentialSets(List<List<GroupCredential>> groupCredentialSets, List<int> userCredentials)
        {
            AnnotatedCredentialSets = groupCredentialSets.Select(gac => gac.Select(gc => new AnnotatedGroupCredential(gc, userCredentials)));
        }

        public IEnumerable<IEnumerable<AnnotatedGroupCredential>> AnnotatedCredentialSets { get; set; }

        public bool IsSatisfied
        {
            get
            {
                return AnnotatedCredentialSets.All(cs => cs.Any(c => c.UserHasCredential));
            }
        }
    }
}
