using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetReportResponse
    {
        [DataMember(Name = "reportData")]
        [JsonPropertyName("reportData")]
        public ReportData GetReportData { get; set; }
        public GetReportResponse(Charts charts, Chart chartModel, ChartTypes chartType)
        {
            GetReportData = new ReportData(charts, chartModel, chartType);
        }
    }
    public class ReportData
    {

        private string GetColour(Charts chart, int index, string label)
        {
            string colour = string.Empty; 
            var colourChart = chart.ColourChart();

            if(colourChart!=null)
            {
                colourChart.TryGetValue(label, out colour);
            }

            if(string.IsNullOrEmpty(colour))
            {
                int colourIndex = index % 10;
                colour =  COLOURS[colourIndex];
            }
            return colour;            
        }

        private readonly Dictionary<int, string> COLOURS = new Dictionary<int, string>()
        {
            {0, ChartHelpers.RED},
            {1, ChartHelpers.ORANGE },
            {2, ChartHelpers.BLUE },
            {3, ChartHelpers.GREEN },
            {4, ChartHelpers.PINK },
            {5, ChartHelpers.YELLOW },
            {6, ChartHelpers.GREY },
            {7,  ChartHelpers.MAROON },
            {8, ChartHelpers.DUCK_EGG},
            {9, ChartHelpers.LIGHT_BLUE },
            {10, ChartHelpers.PURPLE}
        };

        public string type { get; set; }
        public Data data { get; set; }
        public Options options { get; set; }

        private IEnumerable<string> GetLabels(Chart chartModel)
        {
            return chartModel.DataPoints.Select((Value, Index) => (Value, Index)).OrderBy(x => x.Index)
                    .Select(x => x.Value.XAxis)
                    .Distinct();
        }

        public ReportData(Charts charts, Chart chartModel, ChartTypes chartType)
        {
            type = chartType.ToString().ToLower();

            //var labels = chartModel.DataPoints.OrderBy(x => x.XAxis)
            //var labels = chartModel.DataPoints.OrderBy(x => x.XAxis)
            //    .Select(x => x.Series)
            //    .Distinct()
            //    .ToList();

            var labels = chartModel.DataPoints.Select((Value, Index) => (Value, Index)).OrderBy(x => x.Index)
                .Select(x => x.Value.Series)
                .Distinct()
                .ToList();

            List<Dataset> datasets = new List<Dataset>();
            int index = 1;
            labels.ForEach(item =>
            {
                //var dataList = chartModel.DataPoints.Where(x => x.Series == item).OrderBy(o=> o.XAxis).Select(x => (int) x.Value).ToList();                
                var dataList = chartModel.DataPoints.Select((Value, Index) => (Value, Index)).OrderBy(x => x.Index)
                     .Where(x => x.Value.Series == item)
                     .Select(x => (int)x.Value.Value)
                     .ToList();

                List<string> backgroundColor = new List<string>();

                if (chartType == ChartTypes.Pie)
                {
                    for (int i = 1; i <= dataList.Count; i++)
                    {
                        backgroundColor.Add(GetColour(charts, i, item));
                    }
                }
                else
                {
                    backgroundColor.Add(GetColour(charts, index, item));
                }

                datasets.Add(new Dataset
                {
                    label = item,
                    data = dataList.ToArray(),
                    backgroundColor = backgroundColor.ToArray()
                });
                index++;
            });

            //data = new Data() { labels = chartModel.Labels.Select(x => x.ConvertToFriendlyLabel()).ToArray(), datasets = datasets.ToArray() };
            data = new Data() { labels = GetLabels(chartModel).Select(x => x.ConvertToFriendlyLabel()).ToArray(), datasets = datasets.ToArray() };
            options = new Options();
            options.indexAxis = charts.IndexAxis();
            options.plugins = new Plugins()
            {
               title = new Title()
               {
                   display = false,
                  // text = chart.Title
               },
               legend = new Legend()
               {
                   position = "top",
                   display = charts.ShowLegend()
               }
            };
            options.scales = new Scales();
            options.responsive = true;
            if (chartType != ChartTypes.Pie)
            {
                options.scales.yAxes = new Yaxes()
                {
                    grid = new Grid()
                    {
                        display = charts.GridLinesYAxis()
                    },
                    stacked = true,
                    title = new Title1()
                    {
                        display = charts.ShowYAxisName(),
                        text = chartModel.YAxisName,
                        font = new Font
                        {
                            size = 15
                        }
                    }
                };

            }

            if (chartType != ChartTypes.Pie)
            {
                options.scales.xAxes = new Xaxes
                {
                    ticks = new Ticks()
                    {
                        stepSize = 1
                    },
                    grid = new Grid()
                    {
                        display = charts.GridLinesXAxis()
                    },
                    stacked = true,
                    title = new Title2
                    {
                        display = charts.ShowXAxisName(),
                        text = chartModel.XAxisName,
                        font = new Font1
                        {
                            size = 15
                        }
                    }
                };
            }
        }

    }

    public class Data
    {
        public string[] labels { get; set; }
        public Dataset[] datasets { get; set; }
    }

    public class Dataset
    {
        public string label { get; set; }
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }
    }

    public class Options
    {
        public string indexAxis { get; set; }
        public Plugins plugins { get; set; }
        public bool responsive { get; set; }
        public Scales scales { get; set; }
    }

    public class Plugins
    {
        public Title title { get; set; }
        public Legend legend { get; set; }
    }

    public class Legend
    {
        public string position { get; set; }
        public bool display { get; set; }
    }

    public class Title
    {
        public bool display { get; set; }
        public string text { get; set; }
    }

    public class Scales
    {
        public Yaxes yAxes { get; set; }
        public Xaxes xAxes { get; set; }
    }

    public class Yaxes
    {
        public bool stacked { get; set; }
        public Title1 title { get; set; }
        public Grid grid { get; set; }
    }

    public class Grid
    {
        public bool display { get; set; }
    }

    public class Title1
    {
        public bool display { get; set; }
        public string text { get; set; }
        public Font font { get; set; }
    }

    public class Font
    {
        public int size { get; set; }
    }

    public class Xaxes
    {
        public string axis { get; set; }
        public bool stacked { get; set; }
        public Title2 title { get; set; }
        public Grid grid { get; set; }
        public Ticks ticks { get; set; }
    }
    public class Ticks
    {
        public int stepSize { get; set; }
    }

    public class Title2
    {
        public bool display { get; set; }
        public string text { get; set; }
        public Font1 font { get; set; }
    }

    public class Font1
    {
        public int size { get; set; }
    }

}
