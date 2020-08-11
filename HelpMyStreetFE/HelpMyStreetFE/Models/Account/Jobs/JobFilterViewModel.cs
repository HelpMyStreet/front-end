using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterViewModel
    {
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }

        public JobFilterRequest GetJobFilterRequest()
        {
            return new JobFilterRequest()
            {
                SupportActivities = SupportActivities.Where(a => a.IsSelected).Select(a => a.Value),
                JobStatuses = JobStatuses.Where(a => a.IsSelected).Select(a => a.Value),
            };
        }
    }
}
