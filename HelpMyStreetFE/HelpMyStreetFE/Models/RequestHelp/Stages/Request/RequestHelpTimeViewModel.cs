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
        public int Days { get; set; } 
        public bool AllowCustom { get; set; }
        public bool IsSelected { get; set; }
        public bool OnDate { get; set; }
        public DateTime Date { get; set; }
 
    }
}
