using HelpMyStreetFE.Models.Account.Report;
using HelpMyStreetFE.Models.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public async Task<ChartModel> GetReportData(string groupKey)
        {
            ChartModel result = new ChartModel()
            {
                Title = "Activity by month",
                XAxisName = "Month",
                YAxisName = "Count",
                ChartType = "bar"
            };
            List<ReportDataModel> dataItems = new List<ReportDataModel>();
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Friendly Chat", Count = 2 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Other", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-01", Label = "Shopping", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Other", Count = 2 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-02", Label = "Shopping", Count = 2 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Other", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-03", Label = "Shopping", Count = 5 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Other", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-04", Label = "Shopping", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Other", Count = 2 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-05", Label = "Shopping", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Other", Count = 3 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-06", Label = "Shopping", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Other", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-07", Label = "Shopping", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Errands", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Face Covering", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Friendly Chat", Count = 7 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Other", Count = 12 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-08", Label = "Shopping", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Other", Count = 24 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-09", Label = "Shopping", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Other", Count = 6 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Prescriptions", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-10", Label = "Shopping", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Errands", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Friendly Chat", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Other", Count = 12 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-11", Label = "Shopping", Count = 0 });

            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Errands", Count = 1 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Face Covering", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Friendly Chat", Count = 2 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Other", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Prescriptions", Count = 0 });
            dataItems.Add(new ReportDataModel() { XAxis = "2021-12", Label = "Shopping", Count = 3 });

            result.ReportDataModels = dataItems;

            return result;
        }
    }
}
