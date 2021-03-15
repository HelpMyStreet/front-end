using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class FrequencyHelpers
    {
        public static string FriendlyName(this Frequency frequency)
        {
            return frequency switch
            {
                Frequency.Once => "Just once",
                _ => frequency.ToString()
            };
        }
    }
}
