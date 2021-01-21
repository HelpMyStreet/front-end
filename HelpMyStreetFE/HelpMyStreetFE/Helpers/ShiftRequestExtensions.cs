using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Helpers
{
    public static class ShiftRequestExtensions
    {
        public static JobStatuses JobStatus(this RequestSummary shiftRequest)
        {
            // TODO: Request summary status should be some kind of summary of job statuses
            return JobStatuses.InProgress;
        }

        public static string TimeSpan(this ShiftJob shiftJob)
        {
            if (shiftJob.StartDate.Date.Equals(shiftJob.EndDate.Date))
            {
                return $"{shiftJob.StartDate:dd/MM/yyyy HH:mm}-{shiftJob.EndDate:HH:mm}";
            }
            else
            {
                return $"{shiftJob.StartDate:dd/MM/yyyy HH:mm} – {shiftJob.EndDate:dd/MM/yyyy HH:mm}";
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
    }
}
