using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account
{
    public class OpenJobsViewModel
    {
        public IEnumerable<JobSummary> CriteriaJobs { get; set; }
        public IEnumerable<JobSummary> OtherJobs { get; set; }
    }
}
