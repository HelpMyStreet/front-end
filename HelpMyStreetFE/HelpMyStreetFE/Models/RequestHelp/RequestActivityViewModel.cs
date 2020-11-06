using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class RequestActivityViewModel
    {
        public SupportActivities Activity { get; set; }
        public string Description { get; set; }
    }
}
