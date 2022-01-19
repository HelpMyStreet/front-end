using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Report
{
    public class ReportViewModel
    {
        public string Title { get; set; }
        public string ChartType { get; set; }
        public string XAxisName { get; set; }
        public string YAxisName { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public List<ReportItemModel> Data { get; set; }

        public ReportViewModel(Chart chartModel)
        {
            Title = chartModel.Title;
            XAxisName = chartModel.XAxisName;
            YAxisName = chartModel.YAxisName;
            Labels = chartModel.Labels;
            ChartType = chartModel.ChartType.ToString().ToLower();
        }
    }
}
