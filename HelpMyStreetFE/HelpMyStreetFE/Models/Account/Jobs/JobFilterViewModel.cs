using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;
using System.Collections.Generic;
using System;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterViewModel
    {
        public JobSet JobSet { get; set; }
        public int GroupId { get; set; }
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }
        public IEnumerable<FilterField<int>> MaxDistanceInMiles { get; set; }
        public IEnumerable<FilterField<int>> DueInNextXDays { get; set; }
        public Action EmptyJobSetCallback { get; set; }
    }
}
