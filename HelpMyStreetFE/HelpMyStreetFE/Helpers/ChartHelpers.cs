using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers
{
    public static class ChartHelpers
    {
        public static Dictionary<string,string> ColourChart(this Charts chart)
        {
            switch(chart)
            {
                case Charts.RequestVolumeByDueDateAndRecentStatus:
                    return new Dictionary<string, string>()
                    {
                        {"Pending Approval", "rgb(246, 112, 25)" },
                        {"Open","rgb(77, 201, 246)" },
                        {"Accepted","rgb(255, 255, 0)" },
                        {"Done","rgb(172, 194, 54)" },
                        {"Overdue","rgb(245, 55, 148)" },
                        {"Cancelled","rgb(166, 166, 166)" }

                    };
                default:
                    return new Dictionary<string, string>();
            }
        }

        public static string ConvertToFriendlyLabel (this string xAxis)
        {
            DateTime dt;
            DateTime.TryParse(xAxis, out dt);

            if(dt!=null)
            {
               return $"{dt: MMM} '{dt:yy}";
            }
            else
            {
                return xAxis;
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
    }
}
