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
    }
}
