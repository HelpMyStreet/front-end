using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class FrequencyViewModel : BasicTileViewModel
    {
        public FrequencyViewModel(Frequency frequency, bool hideForFaceCoverings)
        {
            DataType = "frequency";
            Frequency = frequency;
            Description = frequency.FriendlyName();
            HideForFaceCoverings = hideForFaceCoverings;
        }

        public Frequency Frequency { get; private set; }

        public bool HideForFaceCoverings { set { if (value) HideTileWhen.Add(new Tuple<string, string>("activity", "FaceMask")); } }

        public int ID
        {
            get
            {
                return (int)Frequency;
            }
        }

        public override string Value
        {
            get
            {
                return Frequency.ToString();
            }
            set { }
        }
    }
}
