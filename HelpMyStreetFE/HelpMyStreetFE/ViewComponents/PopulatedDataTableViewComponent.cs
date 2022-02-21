﻿using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class PopulatedDataTableViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<DataPoint> dataPoints, CancellationToken cancellationToken)
        {
            PopulatedDataTableViewModel viewModel = new PopulatedDataTableViewModel()
            {
                DataPoints = dataPoints
            };            
            return View("PopulatedDataTable", viewModel);
        }
    }
}
