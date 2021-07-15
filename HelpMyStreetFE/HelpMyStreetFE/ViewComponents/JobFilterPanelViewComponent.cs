using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Requests;
using System.Threading;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobFilterPanelViewComponent : ViewComponent
    {
        private readonly IFilterService _filterService;
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;

        public JobFilterPanelViewComponent(IFilterService filterService, IAuthService authService, IRequestService requestService)
        {
            _filterService = filterService;
            _requestService = requestService;
            _authService = authService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterViewModel jobFilterViewModel, CancellationToken cancellationToken)
        {
            if (jobFilterViewModel.FilterSet == null)
            {
                var jobStatuses = new List<JobStatuses>();

                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

                if (jobFilterViewModel.JobFilterRequest.HighlightJobId.HasValue)
                {
                    int jobId = jobFilterViewModel.JobFilterRequest.HighlightJobId.Value;
                    var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);
                    if (job != null)
                    {
                        jobStatuses.Add(job.JobStatus);
                    }
                }

                if (jobFilterViewModel.JobFilterRequest.HighlightRequestId.HasValue)
                {
                    int requestId = jobFilterViewModel.JobFilterRequest.HighlightRequestId.Value;
                    var request = await _requestService.GetRequestSummaryAsync(requestId, cancellationToken);
                    if (request != null)
                    {
                        jobStatuses = request.JobBasics.JobStatusDictionary().Keys.ToList();
                    }
                }

                jobFilterViewModel.FilterSet = await _filterService.GetDefaultSortAndFilterSet(jobFilterViewModel.JobFilterRequest.JobSet, jobFilterViewModel.JobFilterRequest.GroupId, jobStatuses, user, cancellationToken);
            }

            jobFilterViewModel.JobFilterRequest.UpdateFromFilterSet(jobFilterViewModel.FilterSet);

            return View("JobFilterPanel", jobFilterViewModel);
        }
    }
}
