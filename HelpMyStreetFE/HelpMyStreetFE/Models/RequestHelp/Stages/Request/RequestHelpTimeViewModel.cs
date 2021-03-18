using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpTimeViewModel : BasicTileViewModel
    {
        public RequestHelpTimeViewModel()
        {
            DataType = "timeframe";
        }

        public int Days { get; set; }
        public bool HideForRepeatRequests { set { if (value) HideTileWhen.Add("repeats", "true"); } }
        public bool HideForFaceCoverings { set { if (value) HideTileWhen.Add("activity", "FaceMask"); } }
        public DueDateType DueDateType { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration
        {
            get { return EndTime.ToUTCFromUKTime().Subtract(StartTime.ToUTCFromUKTime()); }
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

        public override string Value
        {
            get
            {
                return ID.ToString();
            }
            set { }
        }
    }
}
