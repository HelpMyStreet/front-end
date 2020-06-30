using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models
{
    public class HMSLogoViewModel
    {
        public bool RenderLink { get; set; } = true;

        public bool ShowSecondaryText { get; set; } = false;

        public string PrimaryText { get; set; } = "HelpMyStreet.org";
        public string PrimaryMobileText { get; set; }

        public string SecondaryText { get; set; } = "HelpMyStreet.org";
    }
}
