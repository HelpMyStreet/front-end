using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobLocation
    {
        public JobSet JobSet { get; set; }
        public string GroupKey { get; set; }
    }
}
