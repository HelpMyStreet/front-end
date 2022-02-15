using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
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

        private async Task<Chart> GetChartAndAdddOthersIfNecessary(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo)
        {
            var chartData = await _requestService.GetChart(chart, groupId, dateFrom, dateTo);

            var series = chartData.DataPoints.Select(x => x.Series).Distinct().ToList();

            //Calculate the sum of each value in the series
            Dictionary<string, int> seriesCount = new Dictionary<string, int>();

            series.ForEach(item =>
            {
                seriesCount.Add(item, Convert.ToInt32(chartData.DataPoints.Where(x => x.Series == item).Sum(x => x.Value)));
            });

            //Identify the top 5 most activities
            var ExplicitLegendItems = seriesCount.OrderByDescending(x => x.Value).Take(5).Select(x=> x.Key);

            //Identify all other items
            var OtherItems = chartData.DataPoints.Where(x => !ExplicitLegendItems.Contains(x.Series)).ToList();

            List<DataPoint> otherDataPoints = new List<DataPoint>();

            //Populate other items
            OtherItems
                .Select(x => x.XAxis)
                .Distinct()
                .ToList()
                .ForEach(item =>
                {
                    otherDataPoints.Add(new DataPoint()
                    {
                        XAxis = item,
                        Series = "Others",
                        Value = chartData.DataPoints.Where(x => x.XAxis == item && !ExplicitLegendItems.Contains(x.Series)).Sum(x => x.Value)
                    });
                });

            //remove all other items from the original list
            chartData.DataPoints.RemoveAll(x => !ExplicitLegendItems.Contains(x.Series));

            //add newly created other items
            chartData.DataPoints.AddRange(otherDataPoints);
            
            //replace the datapoints with new datapoints in the correct order           
            chartData.DataPoints = chartData.DataPoints.OrderBy(x => x.XAxis).ToList();

            return chartData;
        }

        private SupportActivityCategory GetCategory(string supportActivity)
        {
            var activity = Enum.GetValues(typeof(SupportActivities)).Cast<SupportActivities>()
                 .ToList()
                 .FirstOrDefault(x => x.FriendlyNameShort() == supportActivity);

            if(activity !=null)
            {
                return activity.Category();
            }
            else
            {
                throw new Exception($"Unable to retrieve category from { supportActivity}");
            }
        }


        private async Task<Chart> GetActivitiesByDateGroupByCategoryIfNecessary(int groupId, DateTime dateFrom, DateTime dateTo)
        {
            var chartData = await _requestService.GetChart(Charts.RequestVolumeByActivityType, groupId, dateFrom, dateTo);

            var series = chartData.DataPoints.Select(x => x.Series).Distinct().ToList();

            if(series.Count>5)
            {
                List<DataPoint> newDataPoints = new List<DataPoint>();

                var categories = Enum.GetValues(typeof(SupportActivityCategory)).Cast<SupportActivityCategory>()
                 .ToList();

                chartData.DataPoints
                    .Select(x => x.XAxis)
                    .Distinct()
                    .ToList()
                    .ForEach(item =>
                    {
                        categories.ForEach(legend =>
                        {
                            var children = chartData.DataPoints
                                .Where(x => GetCategory(x.Series) == legend && x.XAxis == item && x.Value>0)
                                .Select(x => new DataPointChild()
                                {
                                    ChildName = x.Series,
                                    Value = x.Value
                                })
                                .ToList();

                            newDataPoints.Add(new DataPoint()
                            {
                                XAxis = item,
                                Series = legend.ToString(),
                                Value = children.Sum(x=> x.Value),
                                Children = children
                            });
                        });
                    });
                chartData.DataPoints = newDataPoints;
            }
 
            return chartData;
        }
        public async Task<Chart> GetChart(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            switch (chart)
            {
                case Charts.ActivitiesByMonth:
                    return await GetChartAndAdddOthersIfNecessary(chart, groupId, dateFrom, dateTo);
                case Charts.RequestVolumeByActivityType:
                    var data = await GetActivitiesByDateGroupByCategoryIfNecessary(groupId, dateFrom, dateTo);
                    return data;
                case Charts.RequestVolumeByDueDateAndRecentStatus:
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
