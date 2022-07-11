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
        public FrequencyViewModel(Frequency frequency, List<SupportActivities> hideForSupportActivities)
        {
            DataType = "frequency";
            Frequency = frequency;
            Description = frequency.FriendlyName();

            if (hideForSupportActivities != null)
            {
                hideForSupportActivities.ForEach(sa => HideTileWhen.Add(new Tuple<string, string>("activity", sa.ToString())));
            }
        }

        public Frequency Frequency { get; private set; }

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
