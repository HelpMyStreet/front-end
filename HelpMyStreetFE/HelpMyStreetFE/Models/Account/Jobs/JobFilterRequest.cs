using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;
using System;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterRequest
    {
        public JobSet JobSet { get; set; }
        public int? GroupId { get; set; }
        public IEnumerable<SupportActivities> SupportActivities { get; set; }
        public IEnumerable<JobStatuses> JobStatuses { get; set; }
        public int? MaxDistanceInMiles { get; set; }
        public int? DueInNextXDays { get; set; }
        public DateTime? DueAfter { get; set; }
        public DateTime? DueBefore { get; set; }
        public DateTime? RequestedAfter { get; set; }
        public DateTime? RequestedBefore { get; set; }
        public int ResultsToShow { get; set; }
        public int ResultsToShowIncrement { get; set; }
    }
}
