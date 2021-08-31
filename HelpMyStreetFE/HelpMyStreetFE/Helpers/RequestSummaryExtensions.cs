using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class RequestSummaryExtensions
    {
        public static double RequiringAdminAttentionScore(this RequestSummary requestSummary)
        {
            if (requestSummary.JobSummaries.Count() == 0)
            {
                return 0;
            }

            return requestSummary.JobSummaries.Select(j => j.RequiringAdminAttentionScore()).Average();
        }

        public static DateTime NextDueDate(this RequestSummary requestSummary)
        {
            if (requestSummary.Shift != null)
            {
                return requestSummary.Shift.StartDate;
            }
            else if (requestSummary.JobSummaries.Exists(j => j.JobStatus.Incomplete()))
            {
                return requestSummary.JobSummaries.Where(j => j.JobStatus.Incomplete()).Min(j => j.DueDate);
            }
            return DateTime.MaxValue;
        }

        public static DateTime NextDueDate(this RequestSummary requestSummary, int userId)
        {
            if (requestSummary.Shift != null)
            {
                return requestSummary.Shift.StartDate;
            }
            else if (requestSummary.JobSummaries.Exists(j => j.JobStatus.Incomplete() && j.VolunteerUserID.Equals(userId)))
            {
                return requestSummary.JobSummaries.Where(j => j.JobStatus.Incomplete() && j.VolunteerUserID.Equals(userId)).Min(j => j.DueDate);
            }
            return DateTime.MaxValue;
        }
    }
}
