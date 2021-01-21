﻿using System;
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

        public static string SlotJobStatusWithVolunteerName(this JobStatuses jobStatus, UserPersonalDetails userPersonalDetails)
        {
            return jobStatus switch
            {
                JobStatuses.Open => "Vacant",
                JobStatuses.Accepted => $"Accepted by {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                JobStatuses.InProgress => $"In Progress with {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                JobStatuses.Done => $"Completed by {userPersonalDetails?.FirstName} {userPersonalDetails?.LastName}",
                _ => jobStatus.FriendlyName()
            };
        }
    }
}
