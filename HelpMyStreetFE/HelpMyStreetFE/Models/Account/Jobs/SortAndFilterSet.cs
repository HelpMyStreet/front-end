using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class SortAndFilterSet
    {
        public IEnumerable<FilterField<SupportActivities>> SupportActivities { get; set; }
        public IEnumerable<FilterField<JobStatuses>> JobStatuses { get; set; }
        public IEnumerable<FilterField<int>> MaxDistanceInMiles { get; set; }
        public IEnumerable<FilterField<int>> DueInNextXDays { get; set; }
        public IEnumerable<FilterField<Location>> Locations { get; set; }
        public IEnumerable<OrderByField> OrderBy { get; set; }
        public IEnumerable<FilterField<PartOfDay>> PartOfDay { get; set; }
        public int? HighlightJobId { get; set; }
        public int? HighlightRequestId { get; set; }
    }
}
