using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Report
{
    public class ReportViewModel
    {
        public int GroupId { get; set; }
        public Charts Chart { get; set; }
        public ChartTypes ChartType { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
