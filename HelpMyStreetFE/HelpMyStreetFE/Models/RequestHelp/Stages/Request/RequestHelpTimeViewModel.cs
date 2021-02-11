using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpTimeViewModel
    {
        public int ID { get; set; }
        public string TimeDescription { get; set; }
        public int Days { get; set; }
        public bool AllowCustom { get; set; }
        public bool IsSelected { get; set; }
        public DueDateType DueDateType { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration
        {
            get { return EndTime.Subtract(StartTime); }
        }
        public string DurationString
        {
            get
            {
                List<string> components = new List<string>();

                if (Duration.TotalHours >= 1)
                {
                    components.Add($"{Math.Floor(Duration.TotalHours)} hours");
                }
                if (Duration.Minutes > 0)
                {
                    components.Add($"{Duration.Minutes} minutes");
                }

                return string.Join(", ", components);
            }
        }
    }
}
