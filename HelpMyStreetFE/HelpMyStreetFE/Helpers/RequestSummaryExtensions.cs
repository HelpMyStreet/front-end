using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class RequestSummaryExtensions
    {
        public static Dictionary<JobStatuses, int> JobStatusDictionary(this RequestSummary requestSummary)
        {
            return requestSummary.UncancelledJobs().GroupBy(j => j.JobStatus)
                .Select(g => new KeyValuePair<JobStatuses, int>(g.Key, g.Count()))
                .ToDictionary(a => a.Key, a => a.Value);
        }

        public static JobStatuses? SingleJobStatus(this RequestSummary requestSummary)
        {
            return requestSummary.JobStatusDictionary().Count() switch
            {
                0 => JobStatuses.Cancelled,
                1 => requestSummary.JobStatusDictionary().First().Key,
                _ => null
            };
        }

        public static bool RequestComplete(this RequestSummary requestSummary)
        {
            return requestSummary.JobBasics.All(j => j.JobStatus.Complete());
        }

        public static IEnumerable<JobBasic> UncancelledJobs(this RequestSummary requestSummary)
        {
            return requestSummary.JobBasics.Where(j => !j.JobStatus.Equals(JobStatuses.Cancelled));
        }

        public static IEnumerable<JobBasic> IncompleteJobs(this RequestSummary requestSummary)
        {
            return requestSummary.JobBasics.Where(j => j.JobStatus.Incomplete());
        }

        public static IEnumerable<JobBasic> UnfilledJobs(this RequestSummary requestSummary)
        {
            return requestSummary.JobBasics.Where(j => j.JobStatus.Equals(JobStatuses.Open));
        }
    }
}
