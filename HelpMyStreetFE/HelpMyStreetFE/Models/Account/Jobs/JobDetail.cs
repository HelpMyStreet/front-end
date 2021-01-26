using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetail
    {
        public RequestSummary RequestSummary { get; set; }
        public JobSummary JobSummary { get; set; }
        public RequestPersonalDetails Requestor { get; set; }
        public RequestPersonalDetails Recipient { get; set; }
        public List<StatusHistory> JobStatusHistory { get; set; }
        public User CurrentVolunteer { get; set; }
    }
}
