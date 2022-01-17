using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Report
{
    public class ChartModel
    {
        public string Title { get; set; }
        public string ChartType { get; set; }
        public string XAxisName { get; set; }
        public string YAxisName { get; set; }
        public IEnumerable<string> Labels
        {
            get
            {
                return ReportDataModels.OrderBy(x => x.XAxis)
                .Select(x => x.XAxis)
                .Distinct()
                .ToList();
            }
        }
        public List<ReportDataModel> ReportDataModels { get; set; }
    }
}
