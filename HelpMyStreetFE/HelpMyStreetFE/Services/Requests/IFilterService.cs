﻿using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IFilterService
    {
        public SortAndFilterSet GetDefaultSortAndFilterSet(JobSet jobSet, User user);
        IEnumerable<JobHeader> SortAndFilterJobs(IEnumerable<JobHeader> jobs, JobFilterRequest jobFilterRequest);
    }
}
