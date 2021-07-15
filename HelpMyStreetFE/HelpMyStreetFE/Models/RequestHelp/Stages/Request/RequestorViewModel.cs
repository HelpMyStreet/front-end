using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestorViewModel : BasicTileViewModel
    {
        public RequestorViewModel()
        {
            DataType = "request-for";
        }

        public string Text { get; set; }
        public string IconDark { get; set; }
        public string IconLight { get; set; }
        public string ColourCode { get; set; }
        public RequestorType Type { get; set; }
        public override string Value { get { return Type.ToString(); } set { } }
    }
}
