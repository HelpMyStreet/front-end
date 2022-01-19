﻿using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreetFE.Models.Account.Volunteers;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class ReportViewComponent : ViewComponent
    {
        private readonly IReportRepository _reportRepository;
        public ReportViewComponent(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int groupId, Charts chart, CancellationToken cancellationToken)
        {
            Chart chartModel = await _reportRepository.GetChart(chart, groupId, cancellationToken);
            
            ReportViewModel viewModel = new ReportViewModel(chartModel);

            var labels = chartModel.ChartItems.OrderBy(x => x.XAxis)
                .Select(x => x.Label)
                .Distinct()
                .ToList();

            viewModel.Data = new List<ReportItemModel>();
            labels.ForEach(item =>
            {
                var dataList = chartModel.ChartItems.Where(x => x.Label == item).Select(x => x.Count).ToList();
                viewModel.Data.Add(new ReportItemModel()
                {
                    Label = item,
                    DataItems = dataList,
                });
            });


            return View("Report", viewModel);
        }
    }
}
