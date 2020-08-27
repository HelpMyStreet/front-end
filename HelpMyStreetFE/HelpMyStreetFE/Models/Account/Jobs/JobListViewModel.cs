using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobListViewModel
    {
        public IEnumerable<HelpMyStreetFE.Models.Account.Jobs.JobViewModel> Jobs { get; set; }
        public int DisplayedJobs { get { return Jobs.Count(); } }
        public int TotalJobs { get; set; }
    }
}
