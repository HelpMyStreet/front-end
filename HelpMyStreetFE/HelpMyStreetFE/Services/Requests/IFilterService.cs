using System.Collections.Generic;
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
        public Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, int? groupId, JobStatuses? jobStatus, User user, CancellationToken cancellationToken);

        IEnumerable<JobBasic> SortAndFilterJobs(IEnumerable<JobBasic> jobs, JobFilterRequest jobFilterRequest);
        IEnumerable<RequestSummary> SortAndFilterJobs(IEnumerable<RequestSummary> jobs, JobFilterRequest jobFilterRequest);
    }
}
