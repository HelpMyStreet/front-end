using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class ReportDataTableViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int groupId, Charts chart, ChartTypes chartType, string dateFrom, string dateTo, CancellationToken cancellationToken)
        {
            ReportViewModel viewModel = new ReportViewModel()
            {
                Chart = chart,
                GroupId = groupId,
                ChartType = chartType,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            
            return View("ReportDataTable", viewModel);
        }
    }
}
