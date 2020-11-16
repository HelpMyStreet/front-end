using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Enums
{
    public enum Result
    {
        Success,
        Failure_FeedbackAlreadyRecorded,
        Failure_IncorrectJobStatus,
        Failure_ServerError,
        Failure_RequestArchived,
    }
}
