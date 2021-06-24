using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class AcceptJobSeriesPopupViewModel
    {
        public RequestSummary RequestSummary { get; set; }
        public IEnumerable<JobSummary> OpenJobsForUser { get; set; }
        public string ReferringGroup { get; set; }
        public Instructions GroupSupportActivityInstructions { get; set; }
        public RequestType RequestType { get; set; }
    }
}
