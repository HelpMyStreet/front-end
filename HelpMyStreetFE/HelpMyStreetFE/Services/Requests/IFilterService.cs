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
        public Task<SortAndFilterSet> GetDefaultSortAndFilterSet(JobSet jobSet, int? groupId, List<JobStatuses> jobStatuses, User user, CancellationToken cancellationToken);
       
        IEnumerable<ShiftJob> SortAndFilterShiftJobs(IEnumerable<ShiftJob> jobs, JobFilterRequest jobFilterRequest);
        IEnumerable<RequestSummary> SortAndFilterRequests(IEnumerable<RequestSummary> jobs, JobFilterRequest jobFilterRequest);
        IEnumerable<IEnumerable<JobSummary>> SortAndFilterOpenJobs(IEnumerable<IEnumerable<JobSummary>> jobs, JobFilterRequest jfr);
    }
}
