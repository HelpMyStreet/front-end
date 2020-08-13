using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterViewModel
    {
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }
        public IEnumerable<FilterField<int>> MaxDistanceInMiles { get; set; }
        public IEnumerable<FilterField<int>> DueInNextXDays { get; set; }
    }
}
