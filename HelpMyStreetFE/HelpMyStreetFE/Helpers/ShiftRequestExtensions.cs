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
    public static class ShiftRequestExtensions
    {
        public static string DateSpan(this ShiftJob shiftJob)
        {
            if (shiftJob.StartDate.Date.Equals(shiftJob.EndDate.Date))
            {
                return $"{shiftJob.StartDate.ShiftyDate()}";
            }
            else
            {
                return $"{shiftJob.StartDate.ShiftyDate()} - {shiftJob.StartDate.ShiftyDate()}";
            }
        }
        public static string TimeSpan(this ShiftJob shiftJob)
        {

            return $"{shiftJob.StartDate.ShiftyTime()} - {shiftJob.EndDate.ShiftyTime()}";
        }
        public static string DateSpan(this RequestSummary shiftRequest)
        {
            if (shiftRequest.Shift.StartDate.Date.Equals(shiftRequest.Shift.EndDate.Date))
            {
                return $"{shiftRequest.Shift.StartDate.ShiftyDate()}";
            }
            else
            {
                return $"{shiftRequest.Shift.StartDate.ShiftyDate()} - {shiftRequest.Shift.StartDate.ShiftyDate()}";
            }
        }
        public static string TimeSpan(this RequestSummary shiftRequest)
        {
            if (shiftRequest.Shift.StartDate.Date.Equals(shiftRequest.Shift.EndDate.Date))
            {
                return $"{shiftRequest.Shift.StartDate:dd/MM/yyyy HH:mm}-{shiftRequest.Shift.EndDate:HH:mm}";
            }
            else
            {
                return $"{shiftRequest.Shift.StartDate:dd/MM/yyyy HH:mm} – {shiftRequest.Shift.EndDate:dd/MM/yyyy HH:mm}";
            }
        }

        public static Dictionary<JobStatuses, int> JobStatusDictionary(this RequestSummary shiftRequest)
        {
            return shiftRequest.JobSummaries.GroupBy(j => j.JobStatus)
                .Select(g => new KeyValuePair<JobStatuses, int>(g.Key, g.Count()))
                .Where(s => !s.Key.Equals(JobStatuses.Cancelled))
                .ToDictionary(a => a.Key, a => a.Value);
        }

        public static JobStatuses? SingleJobStatus(this RequestSummary shiftRequest)
        {
            return shiftRequest.JobStatusDictionary().Count() switch
            {
                0 => JobStatuses.Cancelled,
                1 => shiftRequest.JobStatusDictionary().First().Key,
                _ => null
            };
        }
    }


    public class ShiftJobDedupe_EqualityComparer : IEqualityComparer<ShiftJob>
    {
        public bool Equals(ShiftJob a, ShiftJob b)
        {
            return a.RequestID == b.RequestID && a.SupportActivity == b.SupportActivity;

        }

        public int GetHashCode([DisallowNull] ShiftJob obj)
        {
            return obj.RequestID.GetHashCode() + obj.SupportActivity.GetHashCode();
        }
    }
}
