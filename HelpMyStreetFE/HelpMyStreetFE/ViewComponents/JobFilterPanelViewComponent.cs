using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services;

namespace HelpMyStreetFE.ViewComponents
{
    public class JobFilterPanelViewComponent : ViewComponent
    {
        private readonly IFilterService _filterService;

        public JobFilterPanelViewComponent(IFilterService filterService)
        {
            _filterService = filterService;
        }

        public async Task<IViewComponentResult> InvokeAsync(JobFilterViewModel jobFilterViewModel)
        {
            if (jobFilterViewModel.FilterSet == null)
            {
                jobFilterViewModel.FilterSet = _filterService.GetDefaultFilterSet(jobFilterViewModel.JobFilterRequest.JobSet, jobFilterViewModel.User);
            }

            return View("JobFilterPanel", jobFilterViewModel);
        }

    }
}
