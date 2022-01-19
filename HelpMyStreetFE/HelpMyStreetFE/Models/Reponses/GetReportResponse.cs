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
            {1, "rgb(255, 99, 132)" },
            {2, "rgb(255, 159, 64)" },
            {3, "rgb(255, 205, 86)" },
            {4, "rgb(75, 192, 192)" },
            {5, "rgb(54, 162, 235)" },
            {6, "rgb(153, 102, 255)"},
            {7, "rgb(201, 203, 207)" },
            {8, "rgb(255, 99, 132)" },
            {9, "rgb(255, 159, 64)" },
            {10, "rgb(255, 205, 86)" },
            {11, "rgb(75, 192, 192)" },
            {12, "rgb(54, 162, 235)" },
            {13, "rgb(153, 102, 255)"},
            {14, "rgb(201, 203, 207)" }
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
                datasets.Add(new Dataset
                {
                    label = item,
                    data = dataList.ToArray(),
                    backgroundColor = COLOURS[index]
                });
                index++;
            });

            data = new Data() { labels = chart.Labels.ToArray(), datasets = datasets.ToArray() };
            options = new Options();
            options.plugins = new Plugins();
            options.scales = new Scales();

            options.plugins.title = new Title()
            {
                display = true,
                text = chart.Title
            };
            options.responsive = true;
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

    public class Data
    {
        public string[] labels { get; set; }
        public Dataset[] datasets { get; set; }
    }

    public class Dataset
    {
        public string label { get; set; }
        public int[] data { get; set; }
        public string backgroundColor { get; set; }
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
