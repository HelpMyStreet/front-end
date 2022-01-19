using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Services.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IRequestService _requestService;

        public ReportRepository(IRequestService requestService)
        {
            _requestService = requestService;
        }
        public async Task<Chart> GetChart(Charts chart, int groupId, CancellationToken cancellationToken)
        {
            switch (chart)
            {
                case Charts.ActivitiesByMonth:
                    return await _requestService.GetChart(chart, groupId);
                default:
                    throw new Exception($"unknown chart {chart}");
            }
        }
    }
}
