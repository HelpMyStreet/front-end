using HelpMyStreetFE.Models.RequestHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpQuestion
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public InputType InputType { get; set; }
        public string Model { get; set; }
        public string Placeholder { get; set; }
    }
}
