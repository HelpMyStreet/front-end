using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class JobStatusHelpers
    {
        public static string Icon(this JobStatuses jobStatus)
        {
            return jobStatus switch
            {
                JobStatuses.Open => "open.svg",
                JobStatuses.InProgress => "inprogress.svg",
                JobStatuses.Done => "complete.svg",
                JobStatuses.Cancelled => "cancelled.svg",
                _ => throw new ArgumentException(message: $"Unexpected JobStatuses value: {jobStatus}", paramName: nameof(jobStatus))
            };
        }

    }
}
