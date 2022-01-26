using HelpMyStreet.Contracts.ReportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetReportResponse
    {
        [DataMember(Name = "reportData")]
        [JsonPropertyName("reportData")]
        public ReportData GetReportData { get; set; }
        public GetReportResponse(Chart chart)
        {
            GetReportData = new ReportData(chart);
        }
    }
    public class ReportData
    {
        private Dictionary<int, string> COLOURS = new Dictionary<int, string>()
        {
            {1, "rgb(246, 112, 25)" },
            {2, "rgb(77, 201, 246)" },
            {3, "rgb(172, 194, 54)" },
            {4, "rgb(245, 55, 148)" },
            {5, "rgb(255, 255, 0)" },
            {6, "rgb(166, 166, 166)" },

            {7, "rgb(255, 99, 132)" },
            {8, "rgb(255, 159, 64)" },
            {9, "rgb(255, 205, 86)" },
            {10, "rgb(75, 192, 192)" },
            {11, "rgb(54, 162, 235)" },
            {12, "rgb(153, 102, 255)"},
            {13, "rgb(201, 203, 207)" },
            {14, "rgb(255, 99, 132)" },
            {15, "rgb(255, 159, 64)" },
            {16, "rgb(255, 205, 86)" },
            {17, "rgb(75, 192, 192)" },
            {18, "rgb(54, 162, 235)" },
            {19, "rgb(153, 102, 255)"},
            {20, "rgb(201, 203, 207)" }
        };

        public string type { get; set; }
        public Data data { get; set; }
        public Options options { get; set; }

        public ReportData(Chart chart)
        {
            type = chart.ChartType.ToString().ToLower();

            var labels = chart.ChartItems.OrderBy(x => x.XAxis)
                .Select(x => x.Label)
                .Distinct()
                .ToList();

            List<Dataset> datasets = new List<Dataset>();
            int index = 1;
            labels.ForEach(item =>
            {
                var dataList = chart.ChartItems.Where(x => x.Label == item).Select(x => x.Count).ToList();
                List<string> backgroundColor = new List<string>();

                if(chart.ChartType == HelpMyStreet.Utils.Enums.ChartTypes.Pie)
                {
                    for(int i = 1; i<= dataList.Count; i++ )
                    {
                        backgroundColor.Add(COLOURS[i]);                        
                    }                    
                }
                else
                {
                    backgroundColor.Add(COLOURS[index]);
                }

                datasets.Add(new Dataset
                {
                    label = item,
                    data = dataList.ToArray(),
                    backgroundColor = backgroundColor.ToArray()
                });
                index++;
            });

            data = new Data() { labels = chart.Labels.ToArray(), datasets = datasets.ToArray() };
            options = new Options();
            options.plugins = new Plugins()
            {
               title = new Title()
               {
                   display = true,
                   text = chart.Title
               },
               legend = new Legend()
               {
                   position = "right"
               }
            };
            options.scales = new Scales();
            options.responsive = true;
            if (chart.ChartType != HelpMyStreet.Utils.Enums.ChartTypes.Pie)
            {
                options.scales.yAxes = new Yaxes()
                {
                    stacked = true,
                    title = new Title1()
                    {
                        display = true,
                        text = chart.YAxisName,
                        font = new Font
                        {
                            size = 15
                        }
                    }
                };

            }

            if (chart.ChartType != HelpMyStreet.Utils.Enums.ChartTypes.Pie)
            {
                options.scales.xAxes = new Xaxes
                {
                    stacked = true,
                    title = new Title2
                    {
                        display = true,
                        text = chart.XAxisName,
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
