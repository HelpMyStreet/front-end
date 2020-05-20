using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account
{
    public class AcceptedRequestsViewModel
    {
        public IEnumerable<JobSummary> Jobs { get; set; }
        public IDictionary<int, RequestContactInformation> ContactInformation { get; set; }
    }
}
