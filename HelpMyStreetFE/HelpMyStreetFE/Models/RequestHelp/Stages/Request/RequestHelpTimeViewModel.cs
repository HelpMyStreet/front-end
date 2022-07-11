using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpTimeViewModel : BasicTileViewModel
    {
        public RequestHelpTimeViewModel()
        {
            DataType = "timeframe";
        }

        public int Days
        {
            set
            {
                StartTime = DateTime.UtcNow.ToUKFromUTCTime().Date.AddDays(value).AddHours(6);
            }
        }

        public bool HideForRepeatRequests { set { if (value) HideTileWhen.Add(new Tuple<string, string>("repeats", "true")); } }

        public List<SupportActivities> HideForSupportActivities
        {
            set { value.ForEach(sa => HideTileWhen.Add(new Tuple<string, string>("activity", sa.ToString()))); }
        }
            
        public DueDateType DueDateType { get; set; }

        private DateTime? startTime;
        public DateTime StartTime
        {
            get
            {
                return (DueDateType, startTime) switch
                {
                    (DueDateType.ASAP, _) => DateTime.UtcNow.ToUKFromUTCTime(),
                    (_, null) => DateTime.MinValue,
                    (_, _) => startTime.Value
                };
            }
            set
            {
                startTime = value;
            }
        }

        public DateTime? EndTime { get; set; }

        public DateTime NotBeforeTime
        {
            get
            {
                return DueDateType switch
                {
                    DueDateType.ASAP => DateTime.UtcNow.ToUKFromUTCTime(),
                    DueDateType.Before => DateTime.UtcNow.ToUKFromUTCTime(),
                    _ => StartTime
                };
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (EndTime.HasValue)
                {
                    return EndTime.Value.ToUTCFromUKTime().Subtract(StartTime.ToUTCFromUKTime());
                }
                return TimeSpan.Zero;
            }
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
