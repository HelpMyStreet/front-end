using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Requests;
using System.Threading;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobFilterPanelViewComponent : ViewComponent
    {
        private readonly IFilterService _filterService;
        private readonly IRequestService _requestService;

        public JobFilterPanelViewComponent(IFilterService filterService, IRequestService requestService)
        {
            _filterService = filterService;
            _requestService = requestService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterViewModel jobFilterViewModel, CancellationToken cancellationToken)
        {
            if (jobFilterViewModel.FilterSet == null)
            {
                JobStatuses? jobStatus = null;

                if (jobFilterViewModel.JobFilterRequest.HighlightJobId.HasValue)
                {
                    int jobId = jobFilterViewModel.JobFilterRequest.HighlightJobId.Value;
                    var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);
                    jobStatus = job?.JobStatus;
                }

                jobFilterViewModel.FilterSet = await _filterService.GetDefaultSortAndFilterSet(jobFilterViewModel.JobFilterRequest.JobSet, jobStatus, jobFilterViewModel.User);
            }

            jobFilterViewModel.JobFilterRequest.UpdateFromFilterSet(jobFilterViewModel.FilterSet);

            return View("JobFilterPanel", jobFilterViewModel);
        }

    }
}
