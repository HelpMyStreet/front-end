using System;
using HelpMyStreetFE.Models.Feedback;
using System.Threading.Tasks;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Contracts;
using HelpMyStreetFE.Models.Account.Report;

namespace HelpMyStreetFE.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<ReportDataModel>> GetReportData(string groupKey);
    }
}
