using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestorViewModel : IBasicTileViewModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string IconDark { get; set; }
        public string IconLight { get; set; }
        public string ColourCode { get; set; }
        public bool IsSelected { get; set; }
        public RequestorType Type { get; set; }

        public string Value { get { return Type.ToString(); } }

        public string DataType { get { return "request-for"; } }
    }
}
