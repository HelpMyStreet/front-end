using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobListViewModel
    {
        public IEnumerable<JobViewModel> Jobs { get; set; }
        public int DisplayedJobs { get { return Jobs.Count(); } }
        public int FilteredJobs { get; set; }
        public int UnfilteredJobs { get; set; }
    }
}
