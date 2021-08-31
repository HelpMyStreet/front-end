using System;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.EqualityComparers;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class IEnumerableJobSummaryExtensions
    {
        public static Dictionary<DateTime, IEnumerable<JobSummary>> GroupByDateAndActivity(this IEnumerable<JobSummary> jobBasics)
        {
            IEqualityComparer<JobBasic> _jobBasicEqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();

            return jobBasics.GroupBy(j => j, _jobBasicEqualityComparer).ToDictionary(g => g.First().DueDate, g => g.AsEnumerable());
        }
    }
}
