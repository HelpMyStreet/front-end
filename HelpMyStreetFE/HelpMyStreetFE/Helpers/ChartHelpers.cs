﻿using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers
{
    public static class ChartHelpers
    {
        public const string RED = "rgb(255, 99, 132)";
        public const string ORANGE = "rgb(255, 159, 64)";
        public const string BLUE = "rgb(77, 201, 246)";
        public const string YELLOW = "rgb(255, 205, 86)";
        public const string GREEN = "rgb(172, 194, 54)";
        public const string PINK = "rgb(245, 55, 148)";
        public const string GREY = "rgb(201, 203, 207)";
        public const string DUCK_EGG = "rgb(75, 192, 192)";
        public const string MAROON = "rgb(102,0,0)";
        public const string LIGHT_BLUE = "rgb(30, 145, 214)";
        public const string DARK_BLUE = "rgb(0, 20, 137)";
        public const string PURPLE = "rgb(153, 102, 255)";

        public static Dictionary<string,string> ColourChart(this Charts chart)
        {
            switch(chart)
            {
                case Charts.RequestVolumeByDueDateAndRecentStatus:
                    return new Dictionary<string, string>()
                    {
                        {"Pending Approval", ORANGE },
                        {"Open", BLUE },
                        {"Accepted", YELLOW },
                        {"Done", GREEN },
                        {"Overdue", PINK },
                        {"Cancelled", GREY }

                    };                
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return new Dictionary<string, string>()
                    {
                        {"Dataset 1",BLUE }
                    };
                default:
                    return new Dictionary<string, string>();
            }
        }


        public static bool ShowXAxisName(this Charts chart)
        {
            return false;
        }

        public static bool ShowYAxisName(this Charts chart)
        {
            return false;
        }

        public static string IndexAxis(this Charts chart)
        {
            switch(chart)
            {
                case Charts.TotalGroupUsersByType:
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return "y";
                default:
                    return "x";
            }
        }

        public static bool ShowLegend(this Charts chart)
        {
            switch (chart)
            {
                case Charts.TotalGroupUsersByType:
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return false;
                default:
                    return true;
            }
        }

        public static bool GridLinesXAxis(this Charts chart)
        {
            switch (chart)
            {
                case Charts.TotalGroupUsersByType:
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return true;
                default:
                    return false;
            }
        }

        public static bool GridLinesYAxis(this Charts chart)
        {
            switch (chart)
            {
                case Charts.TotalGroupUsersByType:
                case Charts.RecentlyActiveVolunteersByVolumeOfAcceptedRequests:
                    return false;
                default:
                    return true;
            }
        }
    }
}

