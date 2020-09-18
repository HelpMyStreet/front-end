using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class OpenJobsViewModel
    {
        public IEnumerable<JobHeader> CriteriaJobs { get; set; }
        public IEnumerable<JobHeader> OtherJobs { get; set; }

        public bool HasNoJobs { get
            {
                return (CriteriaJobs.Count() == 0 && OtherJobs.Count() == 0);
            } 
        }
    }
}
