using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account
{
    public class CredentialsRequiredViewModel
    {
        public AnnotatedGroupActivityCredentialSets AnnotatedGroupActivityCredentialSets { get; set; }
        public JobSummary JobSummary { get; set; }
    }
}
