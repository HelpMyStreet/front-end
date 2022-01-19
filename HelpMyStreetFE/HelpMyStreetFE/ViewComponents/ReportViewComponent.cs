using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Report;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class ReportViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int groupId, Charts chart, int Id, CancellationToken cancellationToken)
        {
            ReportViewModel viewModel = new ReportViewModel()
            {
                Chart = chart,
                GroupId = groupId
            };
            
            return View("Report", viewModel);
        }
    }
}
