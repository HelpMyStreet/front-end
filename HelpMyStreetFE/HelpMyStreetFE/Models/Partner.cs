using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class Partner
    {
        public Groups Group { get; set; }
        public string GroupKey { get; set; }
        public string Label { get; set; }
    }
}
