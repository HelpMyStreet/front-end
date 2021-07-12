using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.EqualityComparers;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class IEnumerableJobBasicExtensions
    {
        public static Dictionary<JobStatuses, int> JobStatusDictionary(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.UncancelledJobs().GroupBy(j => j.JobStatus)
                .Select(g => new KeyValuePair<JobStatuses, int>(g.Key, g.Count()))
                .ToDictionary(a => a.Key, a => a.Value);
        }

        public static JobStatuses? SingleJobStatus(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.JobStatusDictionary().Count() switch
            {
                0 => JobStatuses.Cancelled,
                1 => jobBasics.JobStatusDictionary().First().Key,
                _ => null
            };
        }

        public static bool AllComplete(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.All(j => j.JobStatus.Complete());
        }

        public static IEnumerable<JobBasic> UncancelledJobs(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.Where(j => !j.JobStatus.Equals(JobStatuses.Cancelled));
        }

        public static IEnumerable<JobBasic> IncompleteJobs(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.Where(j => j.JobStatus.Incomplete());
        }

        public static IEnumerable<JobBasic> UnfilledJobs(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.Where(j => j.JobStatus.Equals(JobStatuses.Open));
        }

        public static IEnumerable<JobBasic> AcceptedAndInProgressJobs(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.Where(j => j.JobStatus.Equals(JobStatuses.Accepted) || j.JobStatus.Equals(JobStatuses.InProgress));
        }

        public static Dictionary<DateTime, IEnumerable<JobBasic>> GroupByDateAndActivity(this IEnumerable<JobBasic> jobBasics)
        {
            IEqualityComparer<JobBasic> _jobBasicEqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();

            return jobBasics.GroupBy(j => j, _jobBasicEqualityComparer).ToDictionary(g => g.First().DueDate, g => g.AsEnumerable());
        }

        public static Dictionary<DateTime, IEnumerable<JobSummary>> GroupByDateAndActivity(this IEnumerable<JobSummary> jobBasics)
        {
            IEqualityComparer<JobBasic> _jobBasicEqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();

            return jobBasics.GroupBy(j => j, _jobBasicEqualityComparer).ToDictionary(g => g.First().DueDate, g => g.AsEnumerable());
        }

        public static JobBasic FirstOpenJob(this IEnumerable<JobBasic> jobBasics)
        {
            return jobBasics.Where(j => j.JobStatus.Equals(JobStatuses.Open)).OrderBy(j => j.DueDate).FirstOrDefault();
        }
    }
}
