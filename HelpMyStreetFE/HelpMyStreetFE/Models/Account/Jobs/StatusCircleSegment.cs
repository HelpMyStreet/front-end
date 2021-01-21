using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class StatusCircleSegment
    {
        public string Class { get; set; }
        public double Radius { get; set; }
        public double Proportion { get; set; }
        public double OffsetProportion { get; set; }

        public double Circumference
        {
            get
            {
                return 2 * Radius * Math.PI;
            }
        }

        public string R
        {
            get
            {
                return $"{Radius}rem";
            }
        }

        public string DashArray
        {
            get
            {
                return $"{Proportion * Circumference}rem {Circumference}rem";
            }
        }

        public string DashOffset
        {
            get
            {
                return $"-{OffsetProportion * Circumference}rem";
            }
        }
    }
}
