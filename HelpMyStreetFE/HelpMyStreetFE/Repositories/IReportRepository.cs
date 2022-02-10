using System;
using HelpMyStreetFE.Models.Feedback;
using System.Threading.Tasks;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Contracts;
using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreet.Contracts.ReportService;
using System.Threading;

namespace HelpMyStreetFE.Repositories
{
    public interface IReportRepository
    {
        Task<Chart> GetChart(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    }
}
