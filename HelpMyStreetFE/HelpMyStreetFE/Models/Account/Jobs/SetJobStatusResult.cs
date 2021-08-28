using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class SetJobStatusResult
    {
        public string NewStatus { get; set; }
        public bool RequestFeedback { get; set; }
        public bool LockQuestions { get; set; }
    }
}
