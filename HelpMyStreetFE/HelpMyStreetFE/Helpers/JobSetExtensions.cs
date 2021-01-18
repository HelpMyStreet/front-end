using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;

namespace HelpMyStreetFE.Helpers
{
    public static class JobSetExtensions
    {
        public static RequestType RequestType(this JobSet jobSet)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => HelpMyStreet.Utils.Enums.RequestType.Task,
                JobSet.UserOpenRequests_MatchingCriteria => HelpMyStreet.Utils.Enums.RequestType.Task,
                JobSet.UserOpenRequests_NotMatchingCriteria => HelpMyStreet.Utils.Enums.RequestType.Task,
                JobSet.UserAcceptedRequests => HelpMyStreet.Utils.Enums.RequestType.Task,
                JobSet.UserCompletedRequests => HelpMyStreet.Utils.Enums.RequestType.Task,

                JobSet.UserMyShifts => HelpMyStreet.Utils.Enums.RequestType.Shift,
                JobSet.GroupShifts => HelpMyStreet.Utils.Enums.RequestType.Shift,
                JobSet.UserOpenShifts => HelpMyStreet.Utils.Enums.RequestType.Shift,

                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobSet}", paramName: nameof(jobSet))
            };
        }

        public static bool GroupAdminView(this JobSet jobSet)
        {
            return jobSet switch
            {
                JobSet.GroupRequests => true,
                JobSet.GroupShifts => true,

                JobSet.UserOpenRequests_MatchingCriteria => false,
                JobSet.UserOpenRequests_NotMatchingCriteria => false,
                JobSet.UserAcceptedRequests => false,
                JobSet.UserCompletedRequests => false,

                JobSet.UserMyShifts => false,
                JobSet.UserOpenShifts => false,

                _ => throw new ArgumentException(message: $"Unexpected JobSet value: {jobSet}", paramName: nameof(jobSet))
            };
        }
    }
}
