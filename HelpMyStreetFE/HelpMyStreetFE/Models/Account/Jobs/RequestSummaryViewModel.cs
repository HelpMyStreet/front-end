using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class RequestSummaryViewModel : RequestSummary
    {
        public LocationDetails LocationDetails { get; set; }
    }
}
