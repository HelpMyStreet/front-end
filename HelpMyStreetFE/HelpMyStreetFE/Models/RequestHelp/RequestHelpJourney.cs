using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class RequestHelpJourney
    {
        public RequestHelpFormVariant RequestHelpFormVariant { get; set; }
        public bool AccessRestrictedByRole { get; set; }
        public bool RequestorDefinedByGroup { get; set; }
    }
}
