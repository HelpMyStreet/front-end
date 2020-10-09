using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account
{
    public class AnnotatedGroupActivityCredentialSets
    {
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
