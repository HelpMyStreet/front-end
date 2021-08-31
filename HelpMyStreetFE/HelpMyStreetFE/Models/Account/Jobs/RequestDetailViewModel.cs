using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class RequestDetailViewModel
    {
        public RequestSummary RequestSummary { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public RequestRoles UserRole { get; set; }
        public User User { get; set; }
        public LocationDetails LocationDetails { get; set; }
        public IEnumerable<JobDetail> JobDetails { get; set; }
        public IEnumerable<JobSummary> JobsToShow { get; set; }
        public Dictionary<SupportActivities, Instructions> GroupSupportActivityInstructions { get; set; }
    }
}
