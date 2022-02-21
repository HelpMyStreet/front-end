using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreet.Contracts.ReportService;
using Newtonsoft.Json;
using System;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportAPIController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        public ReportAPIController(IReportRepository reportRepository, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _reportRepository = reportRepository;
            _authService = authService;
            _groupMemberService = groupMemberService;
        }

        [HttpGet("getReport")]
        public async Task<ActionResult<GetReportResponse>> GetReport(Charts chart,int groupId, ChartTypes chartType, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (!await _groupMemberService.GetUserHasRole(user.ID, groupId, GroupRoles.ShowCharts, true, cancellationToken))
            {
                throw new UnauthorizedAccessException("user does not have permission to view charts");
            }

            Chart chartModel = await _reportRepository.GetChart(chart, groupId, dateFrom, dateTo, cancellationToken);        
            GetReportResponse getReportResponse = new GetReportResponse(chart, chartModel, chartType);

            var json = JsonConvert.SerializeObject(getReportResponse);

            return getReportResponse;            
        }

        [HttpGet("getDataTable")]
        public async Task<ActionResult> GetDataTable(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (!await _groupMemberService.GetUserHasRole(user.ID, groupId, GroupRoles.ShowCharts, true, cancellationToken))
            {
                throw new UnauthorizedAccessException("user does not have permission to view charts");
            }

            Chart chartModel = await _reportRepository.GetChart(chart, groupId, dateFrom, dateTo, cancellationToken);
            
            return ViewComponent("PopulatedDataTable", new { chartModel.DataPoints });
        }

    }
}
