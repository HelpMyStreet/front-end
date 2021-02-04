using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account
{
    public class LocationWithDistance
    {
        public Location Location { get; set; }
        public LocationDetails LocationDetails { get; set; }
        public double Distance { get; set; }
    }
}
