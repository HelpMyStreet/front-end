using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class UserDetailsViewModel { 
       public bool OnBehalf { get; set; }
        public string Title { get
            {
                if (OnBehalf)
                {
                    return "Details for the person who needs the help";
                }
                return "Your details";
            } 
        }
        public string NamePostfix
        {
            get
            {
                if (OnBehalf)
                {
                    return "their";
                }
                return "your";
            }
        }

        public string LabelPrefix
        {
            get
            {
                if (OnBehalf)
                {
                    return "Their";
                }
                return "Your";
            }
        }
    }
}
