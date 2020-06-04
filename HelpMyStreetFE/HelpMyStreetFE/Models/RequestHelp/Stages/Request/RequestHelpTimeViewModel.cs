using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpTimeViewModel
    {
        public int ID { get; set; }
        public string TimeDescription { get; set; }
        public int? Days { get; set; } = null;
        public bool AllowCustom { get; set; }
        public bool IsSelected { get; set; }
    }
}
