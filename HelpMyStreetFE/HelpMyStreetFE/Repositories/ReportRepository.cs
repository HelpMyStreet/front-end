using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Services.Groups;
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
        private readonly IGroupService _groupService;

        public ReportRepository(IRequestService requestService, IGroupService groupService )
        {
            _requestService = requestService;
            _groupService = groupService;
        }
        public async Task<Chart> GetChart(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            switch (chart)
            {
                case Charts.ActivitiesByMonth:
                case Charts.RequestVolumeByDueDateAndRecentStatus:
                case Charts.RequestVolumeByActivityType:
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return await _requestService.GetChart(chart, groupId, dateFrom, dateTo);
                case Charts.VolumeOfUsersByType:
                    return await _groupService.GetChart(chart, groupId, dateFrom, dateTo);
                default:
                    throw new Exception($"unknown chart {chart}");
            }
        }
    }
}
