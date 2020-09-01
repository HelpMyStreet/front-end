using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class OpenJobsViewModel
    {
        public IEnumerable<JobSummary> CriteriaJobs { get; set; }
        public IEnumerable<JobSummary> OtherJobs { get; set; }

        public bool HasNoJobs { get
            {
                return (CriteriaJobs.Count() == 0 && OtherJobs.Count() == 0);
            } 
        }
    }
}
