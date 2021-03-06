﻿using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;
using System.Threading.Tasks;
using System.Threading;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IFilterService
    {
        public Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, int? groupId, List<JobStatuses> jobStatuses, User user, CancellationToken cancellationToken);
       
        IEnumerable<JobSummary> SortAndFilterJobs(IEnumerable<JobSummary> jobs, JobFilterRequest jobFilterRequest);
        IEnumerable<ShiftJob> SortAndFilterJobs(IEnumerable<ShiftJob> jobs, JobFilterRequest jobFilterRequest);
        IEnumerable<RequestSummary> SortAndFilterJobs(IEnumerable<RequestSummary> jobs, JobFilterRequest jobFilterRequest);
    }
}
