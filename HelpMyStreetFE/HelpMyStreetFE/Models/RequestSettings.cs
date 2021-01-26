using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Models.Email
{
    public class RequestSettings
    {
        public int ShiftRadius { get; set; }
        public int OpenRequestsRadius { get; set; }
        public List<SupportActivities> NationalSupportActivities { get; set; }
    }
}
