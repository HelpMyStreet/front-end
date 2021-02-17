using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<PartOfDay> PartsOfDay { get; set; }

        public OrderBy OrderBy { get; set; }
        public int? HighlightJobId { get; set; }
        public int? HighlightRequestId { get; set; }

        public int ResultsToShow { get; set; }
        public int ResultsToShowIncrement { get; set; }

        public void UpdateFromFilterSet(SortAndFilterSet filterSet)
        {
            if (filterSet.JobStatuses != null)
            {
                JobStatuses = filterSet.JobStatuses.Where(a => a.IsSelected).Select(a => a.Value);
            }
            if (filterSet.SupportActivities != null)
            {
                SupportActivities = filterSet.SupportActivities.Where(a => a.IsSelected).Select(a => a.Value);
            }
            if (filterSet.Locations != null)
            {
                Locations = filterSet.Locations.Where(a => a.IsSelected).Select(a => a.Value);
            }
            if (filterSet.MaxDistanceInMiles != null)
            {
                MaxDistanceInMiles = filterSet.MaxDistanceInMiles.Where(a => a.IsSelected).First().Value;
            }
            if (filterSet.DueInNextXDays != null)
            {
                DueInNextXDays = filterSet.DueInNextXDays.Where(a => a.IsSelected).First().Value;
            }
            if (filterSet.PartOfDay != null)
            {
                PartsOfDay = filterSet.PartOfDay.Where(a => a.IsSelected).Select(a => a.Value);
            }
            if (filterSet.OrderBy != null)
            {
                OrderBy = filterSet.OrderBy.Where(a => a.IsSelected).First().Value;
            }
            if (filterSet.HighlightJobId != null)
            {
                HighlightJobId = filterSet.HighlightJobId;
            }
            if (filterSet.HighlightRequestId != null)
            {
                HighlightRequestId = filterSet.HighlightRequestId;
            }
        }
    }
}
