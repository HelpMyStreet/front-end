using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterRequest
    {
        public IEnumerable<SupportActivities> SupportActivities { get; set; }
        public IEnumerable<JobStatuses> JobStatuses { get; set; }

    }
}
