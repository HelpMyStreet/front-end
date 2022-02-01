using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using System.Collections.Generic;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.Community;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Users;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreet.Contracts.ReportService;
using Newtonsoft.Json;
using System;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportAPIController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        public ReportAPIController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("getReport")]
        public async Task<ActionResult<GetReportResponse>> GetReport(Charts chart,int groupId, ChartTypes chartType, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            //TODO check user is authorised for this particular group
            Chart chartModel = await _reportRepository.GetChart(chart, groupId, dateFrom, dateTo, cancellationToken);        
            GetReportResponse getReportResponse = new GetReportResponse(chartModel, chartType);

            var json = JsonConvert.SerializeObject(getReportResponse);

            return getReportResponse;            
        }

    }
}
