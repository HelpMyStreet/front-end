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
        public static JobStatuses JobStatus(this ShiftRequest shiftRequest)
        {
            return JobStatuses.InProgress;
        }

        public static string TimeSpan(this ShiftJob shiftJob)
        {
            if (shiftJob.StartDate.Date.Equals(shiftJob.EndDate.Date))
            {
                return $"{shiftJob.StartDate:dd/mm/yyyy HH:mm}-{shiftJob.EndDate:HH:mm}";
            }
            else
            {
                return $"{shiftJob.StartDate:dd/mm/yyyy HH:mm} – {shiftJob.EndDate:dd/mm/yyyy HH:mm}";
            }
        }
    }
}
