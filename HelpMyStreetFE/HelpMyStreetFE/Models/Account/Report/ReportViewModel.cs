﻿using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Report
{
    public class ReportViewModel
    {
        public string Labels { get; set; }
        public List<ReportItemModel> Data { get; set; }
    }
}
