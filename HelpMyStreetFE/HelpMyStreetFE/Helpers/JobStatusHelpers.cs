using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class JobStatusHelpers
    {
        public static string Icon(this JobStatuses jobStatus)
        {
            return jobStatus switch
            {
                JobStatuses.New => "new.svg",
                JobStatuses.Open => "open.svg",
                JobStatuses.Accepted => "accepted.svg",
                JobStatuses.InProgress => "inprogress.svg",
                JobStatuses.Done => "complete.svg",
                JobStatuses.Cancelled => "cancelled.svg",
                _ => throw new ArgumentException(message: $"Unexpected JobStatuses value: {jobStatus}", paramName: nameof(jobStatus))
            };
        }

        public static string Class(this JobStatuses jobStatus)
        {
            return jobStatus switch
            {
                JobStatuses.New => "new",
                JobStatuses.Open => "open",
                JobStatuses.Accepted => "accepted",
                JobStatuses.InProgress => "in-progress",
                JobStatuses.Done => "done",
                JobStatuses.Cancelled => "cancelled",
                _ => throw new ArgumentException(message: $"Unexpected JobStatuses value: {jobStatus}", paramName: nameof(jobStatus))
            };
        }

        public static string SlotJobStatusWithVolunteerName(this JobStatuses jobStatus, UserPersonalDetails userPersonalDetails)
        {
            return jobStatus switch
            {
                JobStatuses.Open => "Open",
                JobStatuses.Accepted => $"Accepted by {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                JobStatuses.InProgress => $"In Progress with {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                JobStatuses.Done => $"Completed by {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                _ => jobStatus.FriendlyName()
            };
        }

        public static int UsualOrderOfProgression(this JobStatuses jobStatus)
        {
            return jobStatus switch
            {
                JobStatuses.New => 10,
                JobStatuses.Open => 20,
                JobStatuses.Accepted => 30,
                JobStatuses.InProgress => 40,
                JobStatuses.Done => 50,
                JobStatuses.Cancelled => 100,
                _ => throw new ArgumentException(message: $"Unexpected JobStatuses value: {jobStatus}", paramName: nameof(jobStatus))
            };
        }
    }
}
