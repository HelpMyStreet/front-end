﻿using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class RequestDetailViewModel
    {
        public RequestRoles UserRole { get; set; }
        public GetRequestDetailsResponse RequestDetail { get; set; }
        public LocationDetails LocationDetails { get; set; }
        public IEnumerable<JobDetail> JobDetails { get; set; }
        public IEnumerable<JobSummary> OpenJobsForUser { get; set; }
        public Dictionary<SupportActivities, Instructions> GroupSupportActivityInstructions { get; set; }
    }
}
