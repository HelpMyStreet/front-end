using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Registration
{
    public class SupportActivityViewModel
    {
        public SupportActivities SupportActivities { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}
