using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class FrequencyViewModel : IBasicTileViewModel
    {
        public Frequency Frequency { get; set; }
        public bool IsSelected { get; set; } 
        
        public string DataType
        {
            get
            {
                return "frequency";
            }
        }

        public int ID
        {
            get
            {
                return (int)Frequency;
            }
        }

        public string Value
        {
            get
            {
                return Frequency.ToString();
            }
        }

        public string Description
        {
            get
            {
                return Frequency.FriendlyName();
            }
        }

        public FrequencyViewModel(Frequency frequency)
        {
            Frequency = frequency;
        }
    }
}
