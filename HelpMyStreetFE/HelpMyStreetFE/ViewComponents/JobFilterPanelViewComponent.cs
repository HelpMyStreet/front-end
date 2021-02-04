using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Requests;
using System.Threading;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Services.Users;

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
                JobStatuses? jobStatus = null;

                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

                if (jobFilterViewModel.JobFilterRequest.HighlightJobId.HasValue)
                {
                    int jobId = jobFilterViewModel.JobFilterRequest.HighlightJobId.Value;
                    var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);
                    jobStatus = job?.JobStatus;
                }

                jobFilterViewModel.FilterSet = await _filterService.GetDefaultSortAndFilterSet(jobFilterViewModel.JobFilterRequest.JobSet, jobFilterViewModel.JobFilterRequest.GroupId, jobStatus, user, cancellationToken);
            }

            jobFilterViewModel.JobFilterRequest.UpdateFromFilterSet(jobFilterViewModel.FilterSet);

            return View("JobFilterPanel", jobFilterViewModel);
        }

    }
}
