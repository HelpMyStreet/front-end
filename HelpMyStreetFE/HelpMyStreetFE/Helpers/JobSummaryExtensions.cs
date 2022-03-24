using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class JobSummaryExtensions
    {
        public static int RequiringAdminAttentionScore(this JobSummary job)
        {
            int daysUntilDueDate = (int)job.DueDate.Date.Subtract(DateTime.UtcNow.Date).TotalDays;
            int daysSinceLastUpdate = (int)DateTime.UtcNow.Date.Subtract(job.DateStatusLastChanged.Date).TotalDays;

            int score = job.JobStatus switch
            {
                JobStatuses.New => 2000,
                JobStatuses.Open => 1000,
                JobStatuses.InProgress => 1000,
                _ => 0
            };

            if (score > 0)
            {
                if (job.IsHealthCritical)
                {
                    score += 50;
                }

                // Show jobs due soonest
                score -= daysUntilDueDate * 10;

                if (daysUntilDueDate < 0)
                {
                    // Overdue
                    if (job.JobStatus == JobStatuses.InProgress && daysSinceLastUpdate < 3)
                    {
                        // ... but recently moved to In Progress
                        score += 100;
                    }
                    else
                    {
                        score += 500;

                        if (job.DueDateType == DueDateType.On)
                        {
                            score += 100;
                        }
                    }
                }
                else if (daysUntilDueDate < 14 && job.JobStatus != JobStatuses.InProgress)
                {
                    // Due soon and not yet In Progress
                    score += 200;

                    if (daysUntilDueDate < 7 && job.DueDateType == DueDateType.On)
                    {
                        score += 50;
                    }
                }
            }
            else
            {
                // Done or Cancelled; show jobs updated most recently
                score -= daysSinceLastUpdate;
            }

            return score;
        }

        public static int GroupSize(this JobSummary job)
        {
            var numberOfAdults = int.Parse(job.Questions.FirstOrDefault(q => q.Id == (int)Questions.GroupSizeAdults)?.Answer ?? "0");
            var numberOfChildren = int.Parse(job.Questions.FirstOrDefault(q => q.Id == (int)Questions.GroupSizeChildren)?.Answer ?? "0");

            return numberOfAdults + numberOfChildren;
        }
    }
}
