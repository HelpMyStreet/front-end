using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobStatusChangePopupViewModel
    {
        public AnnotatedGroupActivityCredentialSets AnnotatedGroupActivityCredentialSets { get; set; }
        public JobSummary JobSummary { get; set; }
        public string ReferringGroup { get; set; }
        public JobStatuses TargetStatus { get; set; }
        public Instructions GroupSupportActivityInstructions { get; set; }
    }
}
