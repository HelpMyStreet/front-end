﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class StatusCircleSegment
    {
        public JobStatuses JobStatus { get; set; }
        public int Value { get; set; }
        public double Radius { get; set; }
        public double Proportion { get; set; }
        public double OffsetProportion { get; set; }

        public double GapProportion { get; set; }

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
                return $"{Radius}px";
            }
        }

        public string DashArray
        {
            get
            {
                return $"0px {OffsetProportion * Circumference}px {(Proportion - GapProportion) * Circumference}px {(1/(OffsetProportion + Proportion)) * Circumference}px";
            }
        }

        public string DashOffset
        {
            get
            {
                return $"0px";
            }
        }
    }
}
